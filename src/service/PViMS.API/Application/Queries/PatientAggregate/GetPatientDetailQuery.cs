using MediatR;
using PVIMS.API.Models;
using System.Runtime.Serialization;

namespace PVIMS.API.Application.Queries.PatientAggregate
{
    [DataContract]
    public class GetPatientDetailQuery
        : IRequest<PatientDetailDto>
    {
        [DataMember]
        public int PatientId { get; private set; }

        public GetPatientDetailQuery()
        {
        }

        public GetPatientDetailQuery(int patientId) : this()
        {
            PatientId = patientId;
        }
    }
}
