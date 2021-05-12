using AutoMapper;
using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PVIMS.API.Helpers;
using PVIMS.API.Infrastructure.Services;
using PVIMS.API.Models;
using PVIMS.Core.CustomAttributes;
using PVIMS.Core.Entities;
using PVIMS.Core.Entities.Keyless;
using PVIMS.Core.Paging;
using PVIMS.Core.Repositories;
using PVIMS.Core.Services;
using PVIMS.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PVIMS.API.Application.Queries.PatientAggregate
{
    public class GetPatientsDetailQueryHandler
        : IRequestHandler<GetPatientsDetailQuery, LinkedCollectionResourceWrapperDto<PatientDetailDto>>
    {
        private readonly IRepositoryInt<CustomAttributeConfiguration> _customAttributeRepository;
        private readonly IRepositoryInt<Facility> _facilityRepository;
        private readonly IRepositoryInt<Patient> _patientRepository;
        private readonly IRepositoryInt<SelectionDataItem> _selectionDataItemRepository;
        private readonly PVIMSDbContext _context;
        private readonly ITypeExtensionHandler _modelExtensionBuilder;
        private readonly ILinkGeneratorService _linkGeneratorService;
        private readonly IMapper _mapper;
        private readonly ILogger<GetPatientsDetailQueryHandler> _logger;

        public GetPatientsDetailQueryHandler(
            IRepositoryInt<CustomAttributeConfiguration> customAttributeRepository,
            IRepositoryInt<Facility> facilityRepository,
            IRepositoryInt<Patient> patientRepository,
            IRepositoryInt<SelectionDataItem> selectionDataItemRepository,
            PVIMSDbContext dbContext,
            ITypeExtensionHandler modelExtensionBuilder,
            ILinkGeneratorService linkGeneratorService,
            IMapper mapper,
            ILogger<GetPatientsDetailQueryHandler> logger)
        {
            _customAttributeRepository = customAttributeRepository ?? throw new ArgumentNullException(nameof(customAttributeRepository));
            _facilityRepository = facilityRepository ?? throw new ArgumentNullException(nameof(facilityRepository));
            _patientRepository = patientRepository ?? throw new ArgumentNullException(nameof(patientRepository));
            _selectionDataItemRepository = selectionDataItemRepository ?? throw new ArgumentNullException(nameof(selectionDataItemRepository));
            _context = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _modelExtensionBuilder = modelExtensionBuilder ?? throw new ArgumentNullException(nameof(modelExtensionBuilder));
            _linkGeneratorService = linkGeneratorService ?? throw new ArgumentNullException(nameof(linkGeneratorService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<LinkedCollectionResourceWrapperDto<PatientDetailDto>> Handle(GetPatientsDetailQuery message, CancellationToken cancellationToken)
        {
            var pagingInfo = new PagingInfo()
            {
                PageNumber = message.PageNumber,
                PageSize = message.PageSize
            };

            var facilityId = await GetFacilityIdAsync(message.FacilityName);
            var custom = await GetCustomAsync(message.CustomAttributeId);

            var facilityIdParm = new SqlParameter("@FacilityID", facilityId.ToString());
            var patientIdParm = new SqlParameter("@PatientID", message.PatientId.ToString());
            var firstNameParm = new SqlParameter("@FirstName", !String.IsNullOrWhiteSpace(message.FirstName) ? (Object)message.FirstName : DBNull.Value);
            var lastNameParm = new SqlParameter("@LastName", !String.IsNullOrWhiteSpace(message.LastName) ? (Object)message.LastName : DBNull.Value);
            var dateOfBirthParm = new SqlParameter("@DateOfBirth", message.DateOfBirth > DateTime.MinValue ? (Object)message.DateOfBirth : DBNull.Value);
            var customAttributeKeyParm = new SqlParameter("@CustomAttributeKey", !String.IsNullOrWhiteSpace(message.CustomAttributeValue) ? (Object)custom.attributeKey : DBNull.Value);
            var customPathParm = new SqlParameter("@CustomPath", !String.IsNullOrWhiteSpace(message.CustomAttributeValue) ? (Object)custom.path : DBNull.Value);
            var customValueParm = new SqlParameter("@CustomValue", !String.IsNullOrWhiteSpace(message.CustomAttributeValue) ? (Object)message.CustomAttributeValue : DBNull.Value);

            var patientsFromRepo = _context.PatientLists
                .FromSqlRaw<PatientList>($"EXECUTE spSearchPatients @FacilityID, @PatientId, @FirstName, @LastName, @DateOfBirth, @CustomAttributeKey, @CustomPath, @CustomValue"
                    , facilityIdParm
                    , patientIdParm
                    , firstNameParm
                    , lastNameParm
                    , dateOfBirthParm
                    , customAttributeKeyParm
                    , customPathParm
                    , customValueParm)
                .AsEnumerable();

            var pagedPatientsFromRepo = PagedCollection<PatientList>.Create(patientsFromRepo, pagingInfo.PageNumber, pagingInfo.PageSize);

            if (pagedPatientsFromRepo != null)
            {
                var mappedPatientsWithLinks = new List<PatientDetailDto>();

                foreach (var pagedPatient in pagedPatientsFromRepo)
                {
                    var mappedPatient = _mapper.Map<PatientDetailDto>(pagedPatient);

                    await CustomMapAsync(pagedPatient, mappedPatient);
                    CreateLinks(mappedPatient);

                    mappedPatientsWithLinks.Add(mappedPatient);
                }

                var wrapper = new LinkedCollectionResourceWrapperDto<PatientDetailDto>(pagedPatientsFromRepo.TotalCount, mappedPatientsWithLinks, pagedPatientsFromRepo.TotalPages);

                CreateLinksForPatients(wrapper, message.OrderBy, message.FacilityName, message.PageNumber, message.PageSize,
                    pagedPatientsFromRepo.HasNext, pagedPatientsFromRepo.HasPrevious);

                return wrapper;
            }

            return null;
        }

        private async Task<int> GetFacilityIdAsync(string facilityName)
        {
            if (!String.IsNullOrWhiteSpace(facilityName))
            {
                var facility = await _facilityRepository.GetAsync(f => f.FacilityName == facilityName);
                if (facility != null)
                {
                    return facility.Id;
                }
            }
            return 0;
        }

        private async Task<(string path, string attributeKey)> GetCustomAsync(int customAttributeId)
        {
            if (customAttributeId > 0)
            {
                var customAttribute = await _customAttributeRepository.GetAsync(ca => ca.Id == customAttributeId);
                if(customAttribute!= null)
                {
                    return (customAttribute.CustomAttributeType == CustomAttributeType.Selection ? "CustomSelectionAttribute" : "CustomStringAttribute", customAttribute.AttributeKey);
                }
            }

            return (string.Empty, string.Empty);
        }

        private async Task CustomMapAsync(PatientList patientListFromRepo, PatientDetailDto mappedPatient)
        {
            if (patientListFromRepo == null)
            {
                throw new ArgumentNullException(nameof(patientListFromRepo));
            }

            if (mappedPatient == null)
            {
                throw new ArgumentNullException(nameof(mappedPatient));
            }

            var patientFromRepo = await _patientRepository.GetAsync(p => p.Id == mappedPatient.Id);
            if (patientFromRepo == null)
            {
                return;
            }

            IExtendable patientExtended = patientFromRepo;

            // Map all custom attributes
            mappedPatient.PatientAttributes = _modelExtensionBuilder.BuildModelExtension(patientExtended)
                .Select(h => new AttributeValueDto()
                {
                    Key = h.AttributeKey,
                    Value = h.Value.ToString(),
                    Category = h.Category,
                    SelectionValue = GetSelectionValue(h.Type, h.AttributeKey, h.Value.ToString())
                }).Where(s => (s.Value != "0" && !String.IsNullOrWhiteSpace(s.Value)) || !String.IsNullOrWhiteSpace(s.SelectionValue)).ToList();

            var attribute = patientExtended.GetAttributeValue("Medical Record Number");
            mappedPatient.MedicalRecordNumber = attribute != null ? attribute.ToString() : "";
        }

        private string GetSelectionValue(CustomAttributeType attributeType, string attributeKey, string selectionKey)
        {
            if(attributeType == CustomAttributeType.Selection)
            {
                var selectionitem = _selectionDataItemRepository.Get(s => s.AttributeKey == attributeKey && s.SelectionKey == selectionKey);

                return selectionitem == null ? string.Empty : selectionitem.Value;
            }

            return "";
        }

        private void CreateLinks(PatientDetailDto mappedPatient)
        {
            mappedPatient.Links.Add(new LinkDto(_linkGeneratorService.CreateResourceUri("Patient", mappedPatient.Id), "self", "GET"));
            mappedPatient.Links.Add(new LinkDto(_linkGeneratorService.CreateNewAppointmentForPatientResourceUri(mappedPatient.Id), "newAppointment", "POST"));
            mappedPatient.Links.Add(new LinkDto(_linkGeneratorService.CreateNewEnrolmentForPatientResourceUri(mappedPatient.Id), "newEnrolment", "POST"));
        }

        private void CreateLinksForPatients(LinkedResourceBaseDto wrapper, 
            string orderBy, string facilityName, int pageNumber, int pageSize, bool hasNext, bool hasPrevious)
        {
            wrapper.Links.Add(
               new LinkDto(
                   _linkGeneratorService.CreatePatientsResourceUri(ResourceUriType.Current, orderBy, facilityName, pageNumber, pageSize),
                   "self", "GET"));

            if (hasNext)
            {
                wrapper.Links.Add(
                   new LinkDto(
                       _linkGeneratorService.CreatePatientsResourceUri(ResourceUriType.NextPage, orderBy, facilityName, pageNumber, pageSize),
                       "nextPage", "GET"));
            }

            if (hasPrevious)
            {
                wrapper.Links.Add(
                   new LinkDto(
                       _linkGeneratorService.CreatePatientsResourceUri(ResourceUriType.PreviousPage, orderBy, facilityName, pageNumber, pageSize),
                       "previousPage", "GET"));
            }
        }
    }
}
