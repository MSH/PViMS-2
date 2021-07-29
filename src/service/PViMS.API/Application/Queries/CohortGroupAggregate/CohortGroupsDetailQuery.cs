﻿using MediatR;
using PVIMS.API.Models;
using System.Runtime.Serialization;

namespace PVIMS.API.Application.Queries.CohortGroupAggregate
{
    [DataContract]
    public class CohortGroupsDetailQuery
        : IRequest<LinkedCollectionResourceWrapperDto<CohortGroupDetailDto>>
    {
        [DataMember]
        public string OrderBy { get; private set; }

        [DataMember]
        public int PageNumber { get; private set; }

        [DataMember]
        public int PageSize { get; private set; }

        public CohortGroupsDetailQuery()
        {
        }

        public CohortGroupsDetailQuery(string orderBy, int pageNumber, int pageSize) : this()
        {
            OrderBy = orderBy;
            PageNumber = pageNumber;
            PageSize = pageSize;
        }
    }
}
