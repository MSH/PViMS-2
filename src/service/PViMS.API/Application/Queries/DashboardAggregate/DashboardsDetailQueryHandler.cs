using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using PVIMS.API.Application.Queries.DashboardAggregate;
using PVIMS.API.Helpers;
using PVIMS.API.Infrastructure.Services;
using PVIMS.API.Models;
using PVIMS.Core.Aggregates.DashboardAggregate;
using PVIMS.Core.Paging;
using PVIMS.Core.Repositories;
using Extensions = PVIMS.Core.Utilities.Extensions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PVIMS.API.Application.Queries.EncounterAggregate
{
    public class DashboardsDetailQueryHandler
        : IRequestHandler<DashboardsDetailQuery, LinkedCollectionResourceWrapperDto<DashboardDetailDto>>
    {
        private readonly IRepositoryInt<Dashboard> _dashboardRepository;
        private readonly ILinkGeneratorService _linkGeneratorService;
        private readonly IMapper _mapper;
        private readonly ILogger<DashboardsDetailQueryHandler> _logger;

        public DashboardsDetailQueryHandler(
            IRepositoryInt<Dashboard> dashboardRepository,
            ILinkGeneratorService linkGeneratorService,
            IMapper mapper,
            ILogger<DashboardsDetailQueryHandler> logger)
        {
            _dashboardRepository = dashboardRepository ?? throw new ArgumentNullException(nameof(dashboardRepository));
            _linkGeneratorService = linkGeneratorService ?? throw new ArgumentNullException(nameof(linkGeneratorService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<LinkedCollectionResourceWrapperDto<DashboardDetailDto>> Handle(DashboardsDetailQuery message, CancellationToken cancellationToken)
        {
            var pagingInfo = new PagingInfo()
            {
                PageNumber = message.PageNumber,
                PageSize = message.PageSize
            };

            var orderby = Extensions.GetOrderBy<Dashboard>(message.OrderBy, "asc");

            var pagedDashboardsFromRepo = await _dashboardRepository.ListAsync(pagingInfo, null, orderby, new string[] { });
            if (pagedDashboardsFromRepo != null)
            {
                // Map EF entity to Dto
                var mappedDashboards = PagedCollection<DashboardDetailDto>.Create(_mapper.Map<PagedCollection<DashboardDetailDto>>(pagedDashboardsFromRepo),
                    pagingInfo.PageNumber,
                    pagingInfo.PageSize,
                    pagedDashboardsFromRepo.TotalCount);

                var wrapper = new LinkedCollectionResourceWrapperDto<DashboardDetailDto>(pagedDashboardsFromRepo.TotalCount, mappedDashboards, pagedDashboardsFromRepo.TotalPages);

                CreateLinksForDashboards(wrapper, message.OrderBy, message.PageNumber, message.PageSize,
                    pagedDashboardsFromRepo.HasNext, pagedDashboardsFromRepo.HasPrevious);

                return wrapper;

            }

            return null;
        }

        private void CreateLinksForDashboards(
            LinkedResourceBaseDto wrapper,
            string orderBy,
            int pageNumber, int pageSize,
            bool hasNext, bool hasPrevious)
        {
            wrapper.Links.Add(
               new LinkDto(
                   _linkGeneratorService.CreateDashboardsResourceUri(ResourceUriType.Current, orderBy, pageNumber, pageSize),
                   "self", "GET"));

            if (hasNext)
            {
                wrapper.Links.Add(
                   new LinkDto(
                       _linkGeneratorService.CreateDashboardsResourceUri(ResourceUriType.NextPage, orderBy, pageNumber, pageSize),
                       "nextPage", "GET"));
            }

            if (hasPrevious)
            {
                wrapper.Links.Add(
                   new LinkDto(
                       _linkGeneratorService.CreateDashboardsResourceUri(ResourceUriType.PreviousPage, orderBy, pageNumber, pageSize),
                       "previousPage", "GET"));
            }
        }
    }
}
