using MediatR;
using System.Runtime.Serialization;

namespace PVIMS.API.Application.Commands.PatientAggregate
{
    [DataContract]
    public class ProcessFollowUpFormCommand
        : IRequest<bool>
    {
        [DataMember]
        public int PatientId { get; private set; }

        [DataMember]
        public string Notes { get; set; }

        public ProcessFollowUpFormCommand()
        {
        }

        public ProcessFollowUpFormCommand(int patientId, string notes) : this()
        {
            PatientId = patientId;
            Notes = notes;
        }
    }
}