﻿using System.Runtime.Serialization;

namespace PVIMS.API.Models
{
    /// <summary>
    /// An appointment representation DTO - FULL DETAILS
    /// </summary>
    [DataContract()]
    public class AppointmentDetailDto : AppointmentIdentifierDto
    {
        /// <summary>
        /// The reason for the appointment
        /// </summary>
        [DataMember]
        public string Reason { get; set; }

        /// <summary>
        /// Has the appointment been marked as DNA
        /// </summary>
        [DataMember]
        public bool DidNotArrive { get; set; }

        /// <summary>
        /// Has the appointment been marked as cancelled
        /// </summary>
        [DataMember]
        public string Cancelled { get; set; }

        /// <summary>
        /// The reason for the cancellation
        /// </summary>
        [DataMember]
        public string CancellationReason { get; set; }

        /// <summary>
        /// Details of the household creation
        /// </summary>
        [DataMember]
        public string CreatedDetail { get; set; }

        /// <summary>
        /// Details of the last update to the household
        /// </summary>
        [DataMember]
        public string UpdatedDetail { get; set; }
    }

}
