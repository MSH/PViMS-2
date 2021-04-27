using System.Runtime.Serialization;

namespace PVIMS.API.Models
{
    /// <summary>
    /// A dto representing the output for an adverse event report
    /// </summary>
    [DataContract()]
    public class AdverseEventReportDto : LinkedResourceBaseDto
    {
        /// <summary>
        /// The adverse event being reported on
        /// </summary>
        [DataMember]
        public string AdverseEvent { get; set; }

        /// <summary>
        /// Stratification criteris
        /// </summary>
        [DataMember]
        public string Criteria { get; set; }

        /// <summary>
        /// Stratification by is serious
        /// </summary>
        [DataMember]
        public string Serious { get; set; }

        /// <summary>
        /// The total number of patients meeting the criteria
        /// </summary>
        [DataMember]
        public int PatientCount { get; set; }
    }

}
