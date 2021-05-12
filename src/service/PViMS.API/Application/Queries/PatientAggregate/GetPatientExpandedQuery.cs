using MediatR;
using PVIMS.API.Models;
using System.Runtime.Serialization;

namespace PVIMS.API.Application.Queries.PatientAggregate
{
    [DataContract]
    public class GetPatientExpandedQuery
        : IRequest<PatientExpandedDto>
    {
        [DataMember]
        public int PatientId { get; private set; }

        public GetPatientExpandedQuery()
        {
        }

        public GetPatientExpandedQuery(int patientId) : this()
        {
            PatientId = patientId;
        }
    }
}
