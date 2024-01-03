using MediatR;
using System.Runtime.Serialization;

namespace PVIMS.API.Application.Commands.PatientAggregate
{
    [DataContract]
    public class ProcessBaselineFormCommand
        : IRequest<bool>
    {
        [DataMember]
        public int PatientId { get; private set; }

        [DataMember]
        public string Notes { get; set; }

        public ProcessBaselineFormCommand()
        {
        }

        public ProcessBaselineFormCommand(int patientId, string notes) : this()
        {
            PatientId = patientId;
            Notes = notes;
        }
    }
}