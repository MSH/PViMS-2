using MediatR;
using System.Runtime.Serialization;

namespace PVIMS.API.Application.Commands.PatientAggregate
{
    [DataContract]
    public class ArchivePatientConditionCommand
        : IRequest<bool>
    {
        [DataMember]
        public int PatientId { get; private set; }

        [DataMember]
        public int PatientConditionId { get; private set; }

        [DataMember]
        public string Reason { get; private set; }


        public ArchivePatientConditionCommand()
        {
        }

        public ArchivePatientConditionCommand(int patientId, int patientConditionId, string reason) : this()
        {
            PatientId = patientId;
            PatientConditionId = patientConditionId;
            Reason = reason;
        }
    }
}