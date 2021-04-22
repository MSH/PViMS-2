﻿using System.Collections.Generic;
using System.Runtime.Serialization;

namespace PVIMS.API.Models
{
    /// <summary>
    /// A patient clinical event representation DTO - EXPANDED DETAILS
    /// </summary>
    [DataContract()]
    public class PatientClinicalEventExpandedDto : PatientClinicalEventDetailDto
    {
        /// <summary>
        /// The MedDra term as set by the PV specialist
        /// </summary>
        [DataMember]
        public string SetMedDraTerm { get; set; }

        /// <summary>
        /// Medications which have causality set
        /// </summary>
        [DataMember]
        public ICollection<ReportInstanceMedicationDetailDto> Medications { get; set; } = new List<ReportInstanceMedicationDetailDto>();

        /// <summary>
        /// All activity for this reporting instance
        /// </summary>
        [DataMember]
        public ICollection<ActivityExecutionStatusEventDto> Activity { get; set; } = new List<ActivityExecutionStatusEventDto>();
    }
}
