using MediatR;
using PVIMS.API.Application.Models;
using System.Runtime.Serialization;

namespace PVIMS.API.Application.Queries.WorkFlowAggregate
{
    [DataContract]
    public class WorkFlowDownloadActiveDatasetQuery
        : IRequest<ArtifactDto>
    {
        [DataMember]
        public long CohortGroupId { get; private set; }

        public WorkFlowDownloadActiveDatasetQuery()
        {
        }

        public WorkFlowDownloadActiveDatasetQuery(long cohortGroupId) : this()
        {
            CohortGroupId = cohortGroupId;
        }
    }
}
