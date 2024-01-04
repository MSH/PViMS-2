using MediatR;
using PVIMS.API.Models;
using System.Runtime.Serialization;

namespace PVIMS.API.Application.Queries.PatientAggregate
{
    [DataContract]
    public class PatientExpandedByCustomAttributeQuery
        : IRequest<PatientExpandedDto>
    {
        [DataMember]
        public int CohortGroupId { get; private set; }

        [DataMember]
        public string MedicalRecordNumber { get; private set; }

        public PatientExpandedByCustomAttributeQuery()
        {
        }

        public PatientExpandedByCustomAttributeQuery(int cohortGroupId, string medicalRecordNumber) : this()
        {
            CohortGroupId = cohortGroupId;
            MedicalRecordNumber = medicalRecordNumber;
        }
    }
}