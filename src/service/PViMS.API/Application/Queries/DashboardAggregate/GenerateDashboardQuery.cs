using MediatR;
using PVIMS.API.Models;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace PVIMS.API.Application.Queries.DashboardAggregate
{
    [DataContract]
    public class GenerateDashboardQuery
        : IRequest<List<ChartDto>>
    {
        [DataMember]
        public long DashboardId { get; private set; }

        public GenerateDashboardQuery()
        {
        }

        public GenerateDashboardQuery(long dashboardId) : this()
        {
            DashboardId = dashboardId;
        }
    }
}