using System.Runtime.Serialization;

namespace PVIMS.API.Models
{
    /// <summary>
    /// An appointment representation DTO - ENCOUNTER SEARCH
    /// </summary>
    [DataContract()]
    public class AppointmentSearchDto : AppointmentIdentifierDto
    {
        /// <summary>
        /// The reason for the appointment
        /// </summary>
        [DataMember]
        public string Reason { get; set; }

        /// <summary>
        /// The current status of the appointment
        /// </summary>
        [DataMember]
        public string AppointmentStatus { get; set; }

        /// <summary>
        /// The first name of the patient
        /// </summary>
        [DataMember]
        public string FirstName { get; set; }

        /// <summary>
        /// The last name of the patient
        /// </summary>
        [DataMember]
        public string LastName { get; set; }

        /// <summary>
        /// The current facility of the patient
        /// </summary>
        [DataMember]
        public string CurrentFacility { get; set; }

        /// <summary>
        /// The unique id of the encounter if an encounter has been created
        /// </summary>
        [DataMember]
        public int EncounterId { get; set; }
    }
}
