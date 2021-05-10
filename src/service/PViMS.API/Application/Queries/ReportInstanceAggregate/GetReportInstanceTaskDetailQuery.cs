﻿using MediatR;
using PVIMS.API.Models;
using System;
using System.Runtime.Serialization;

namespace PVIMS.API.Application.Queries.ReportInstanceAggregate
{
    [DataContract]
    public class GetReportInstanceTaskDetailQuery
        : IRequest<TaskDto>
    {
        [DataMember]
        public Guid WorkFlowGuid { get; private set; }

        [DataMember]
        public int ReportInstanceId { get; private set; }

        [DataMember]
        public int Id { get; private set; }

        public GetReportInstanceTaskDetailQuery()
        {
        }

        public GetReportInstanceTaskDetailQuery(Guid workFlowGuid, int reportInstanceId, int id) : this()
        {
            WorkFlowGuid = workFlowGuid;
            ReportInstanceId = reportInstanceId;
            Id = id;
        }
    }
}
