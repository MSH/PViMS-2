using MediatR;
using PVIMS.API.Models;
using System.Runtime.Serialization;

namespace PVIMS.API.Application.Queries.EncounterAggregate
{
    [DataContract]
    public class GetEncounterExpandedQuery
        : IRequest<EncounterExpandedDto>
    {
        [DataMember]
        public int PatientId { get; private set; }

        [DataMember]
        public int EncounterId { get; private set; }

        public GetEncounterExpandedQuery()
        {
        }

        public GetEncounterExpandedQuery(int patientId, int encounterId) : this()
        {
            PatientId = patientId;
            EncounterId = encounterId;
        }
    }
}
