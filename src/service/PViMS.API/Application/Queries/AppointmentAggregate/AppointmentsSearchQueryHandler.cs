using MediatR;
using Microsoft.Extensions.Logging;
using PVIMS.API.Helpers;
using PVIMS.API.Infrastructure.Services;
using PVIMS.API.Models;
using PVIMS.Core.CustomAttributes;
using PVIMS.Core.Entities;
using PVIMS.Core.Paging;
using PVIMS.Core.Repositories;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PVIMS.API.Application.Queries.AppointmentAggregate
{
    public class AppointmentsSearchQueryHandler
        : IRequestHandler<AppointmentsSearchQuery, LinkedCollectionResourceWrapperDto<AppointmentSearchDto>>
    {
        private readonly IAppointmentQueries _appointmentQueries;
        private readonly IRepositoryInt<CustomAttributeConfiguration> _customAttributeRepository;
        private readonly IRepositoryInt<Facility> _facilityRepository;
        private readonly ILinkGeneratorService _linkGeneratorService;
        private readonly ILogger<AppointmentsSearchQueryHandler> _logger;

        public AppointmentsSearchQueryHandler(
            IAppointmentQueries appointmentQueries,
            IRepositoryInt<CustomAttributeConfiguration> customAttributeRepository,
            IRepositoryInt<Facility> facilityRepository,
            ILinkGeneratorService linkGeneratorService,
            ILogger<AppointmentsSearchQueryHandler> logger)
        {
            _appointmentQueries = appointmentQueries ?? throw new ArgumentNullException(nameof(appointmentQueries));
            _customAttributeRepository = customAttributeRepository ?? throw new ArgumentNullException(nameof(customAttributeRepository));
            _facilityRepository = facilityRepository ?? throw new ArgumentNullException(nameof(facilityRepository));
            _linkGeneratorService = linkGeneratorService ?? throw new ArgumentNullException(nameof(linkGeneratorService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<LinkedCollectionResourceWrapperDto<AppointmentSearchDto>> Handle(AppointmentsSearchQuery message, CancellationToken cancellationToken)
        {
            var pagingInfo = new PagingInfo()
            {
                PageNumber = message.PageNumber,
                PageSize = message.PageSize
            };

            Facility facility = null;
            if (!String.IsNullOrWhiteSpace(message.FacilityName))
            {
                facility = await _facilityRepository.GetAsync(f => f.FacilityName == message.FacilityName);
            }

            if (String.IsNullOrWhiteSpace(message.CustomAttributeValue))
            {
                var customAttribute = await _customAttributeRepository.GetAsync(ca => ca.Id == message.CustomAttributeId);
                var path = customAttribute?.CustomAttributeType == CustomAttributeType.Selection ? "CustomSelectionAttribute" : "CustomStringAttribute";

                var results = await _appointmentQueries.GetAppointmentsUsingPatientAttributeAsync(
                    (int)message.CriteriaId,
                    message.SearchFrom,
                    message.SearchTo,
                    facility != null ? facility.Id : 0,
                    message.PatientId,
                    message.FirstName,
                    message.LastName,
                    customAttribute?.AttributeKey,
                    path,
                    message.CustomAttributeValue);
                var pagedResults = PagedCollection<AppointmentSearchDto>.Create(results, pagingInfo.PageNumber, pagingInfo.PageSize);

                var wrapper = new LinkedCollectionResourceWrapperDto<AppointmentSearchDto>(pagedResults.TotalCount, pagedResults);

                CreateLinksForAppointments(wrapper, message.PageNumber, message.PageSize, pagedResults.HasNext, pagedResults.HasPrevious);

                return wrapper;
            }

            return null;
        }

        private LinkedResourceBaseDto CreateLinksForAppointments(
            LinkedResourceBaseDto wrapper,
            int pageNumber, int pageSize,
            bool hasNext, bool hasPrevious)
        {
            wrapper.Links.Add(
               new LinkDto(
                   _linkGeneratorService.CreateAppointmentsResourceUri(ResourceUriType.Current, pageNumber, pageSize),
                   "self", "GET"));

            if (hasNext)
            {
                wrapper.Links.Add(
                   new LinkDto(
                       _linkGeneratorService.CreateAppointmentsResourceUri(ResourceUriType.NextPage, pageNumber, pageSize),
                       "nextPage", "GET"));
            }

            if (hasPrevious)
            {
                wrapper.Links.Add(
                   new LinkDto(
                       _linkGeneratorService.CreateAppointmentsResourceUri(ResourceUriType.PreviousPage, pageNumber, pageSize),
                       "previousPage", "GET"));
            }

            return wrapper;
        }
    }
}
