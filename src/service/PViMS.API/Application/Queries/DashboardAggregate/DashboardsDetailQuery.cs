using MediatR;
using PVIMS.API.Models;
using System.Runtime.Serialization;

namespace PVIMS.API.Application.Queries.DashboardAggregate
{
    [DataContract]
    public class DashboardsDetailQuery
        : IRequest<LinkedCollectionResourceWrapperDto<DashboardDetailDto>>
    {
        [DataMember]
        public string OrderBy { get; private set; }

        [DataMember]
        public int PageNumber { get; private set; }

        [DataMember]
        public int PageSize { get; private set; }

        public DashboardsDetailQuery()
        {
        }

        public DashboardsDetailQuery(string orderBy, int pageNumber, int pageSize) : this()
        {
            OrderBy = orderBy;
            PageNumber = pageNumber;
            PageSize = pageSize;
        }
    }
}