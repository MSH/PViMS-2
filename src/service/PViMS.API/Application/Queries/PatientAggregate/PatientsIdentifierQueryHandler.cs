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
using PVIMS.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PVIMS.API.Application.Queries.PatientAggregate
{
    public class PatientsIdentifierQueryHandler
        : IRequestHandler<PatientsIdentifierQuery, LinkedCollectionResourceWrapperDto<PatientIdentifierDto>>
    {
        private readonly IRepositoryInt<CustomAttributeConfiguration> _customAttributeRepository;
        private readonly IRepositoryInt<Facility> _facilityRepository;
        private readonly PVIMSDbContext _context;
        private readonly ILinkGeneratorService _linkGeneratorService;
        private readonly IMapper _mapper;
        private readonly ILogger<PatientsIdentifierQueryHandler> _logger;

        public PatientsIdentifierQueryHandler(
            IRepositoryInt<CustomAttributeConfiguration> customAttributeRepository,
            IRepositoryInt<Facility> facilityRepository,
            PVIMSDbContext dbContext,
            ILinkGeneratorService linkGeneratorService,
            IMapper mapper,
            ILogger<PatientsIdentifierQueryHandler> logger)
        {
            _customAttributeRepository = customAttributeRepository ?? throw new ArgumentNullException(nameof(customAttributeRepository));
            _facilityRepository = facilityRepository ?? throw new ArgumentNullException(nameof(facilityRepository));
            _context = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _linkGeneratorService = linkGeneratorService ?? throw new ArgumentNullException(nameof(linkGeneratorService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<LinkedCollectionResourceWrapperDto<PatientIdentifierDto>> Handle(PatientsIdentifierQuery message, CancellationToken cancellationToken)
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
                var mappedPatientsWithLinks = new List<PatientIdentifierDto>();

                foreach (var pagedPatient in pagedPatientsFromRepo)
                {
                    var mappedPatient = _mapper.Map<PatientIdentifierDto>(pagedPatient);
                    CreateLinks(mappedPatient);

                    mappedPatientsWithLinks.Add(mappedPatient);
                }

                var wrapper = new LinkedCollectionResourceWrapperDto<PatientIdentifierDto>(pagedPatientsFromRepo.TotalCount, mappedPatientsWithLinks, pagedPatientsFromRepo.TotalPages);

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

        private void CreateLinks(PatientIdentifierDto mappedPatient)
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
