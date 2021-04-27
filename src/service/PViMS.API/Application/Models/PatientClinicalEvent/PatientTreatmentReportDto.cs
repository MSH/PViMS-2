using System.Collections.Generic;
using System.Runtime.Serialization;

namespace PVIMS.API.Models
{
    /// <summary>
    /// A dto representing the output for a patient on treatment report
    /// </summary>
    [DataContract()]
    public class PatientTreatmentReportDto : LinkedResourceBaseDto
    {
        /// <summary>
        /// The unique identifier for the facility
        /// </summary>
        [DataMember]
        public int FacilityId { get; set; }

        /// <summary>
        /// The name of the facility
        /// </summary>
        [DataMember]
        public string FacilityName { get; set; }

        /// <summary>
        /// The total number of patients
        /// </summary>
        [DataMember]
        public int PatientCount { get; set; }

        /// <summary>
        /// The total number of patients with a non serious event
        /// </summary>
        [DataMember]
        public int PatientWithNonSeriousEventCount { get; set; }

        /// <summary>
        /// The total number of patients with a serious event
        /// </summary>
        [DataMember]
        public int PatientWithSeriousEventCount { get; set; }

        /// <summary>
        /// The total number of patients with an event
        /// </summary>
        [DataMember]
        public int PatientWithEventCount { get; set; }

        /// <summary>
        /// Percentage of patients with an event
        /// </summary>
        [DataMember]
        public decimal EventPercentage { get; set; }

        /// <summary>
        /// List of patients for selected facility
        /// </summary>
        [DataMember]
        public List<PatientListDto> Patients { get; set; }
    }

}
