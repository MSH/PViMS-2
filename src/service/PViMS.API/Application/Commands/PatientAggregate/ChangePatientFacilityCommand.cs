using MediatR;
using System.Runtime.Serialization;

namespace PVIMS.API.Application.Commands.PatientAggregate
{
    [DataContract]
    public class ChangePatientFacilityCommand
        : IRequest<bool>
    {
        [DataMember]
        public int PatientId { get; private set; }

        [DataMember]
        public int FacilityId { get; private set; }

        public ChangePatientFacilityCommand()
        {
        }

        public ChangePatientFacilityCommand(int patientId, int facilityId) : this()
        {
            PatientId = patientId;
            FacilityId = facilityId;
        }
    }
}