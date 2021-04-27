using System.Runtime.Serialization;

namespace PVIMS.API.Models
{
    /// <summary>
    /// A work flow representation DTO - FULL DETAILS
    /// </summary>
    [DataContract()]
    public class WorkFlowDetailDto : WorkFlowIdentifierDto
    {
        /// <summary>
        /// The number of new reports for this work flow
        /// </summary>
        [DataMember]
        public int NewReportInstanceCount { get; set; }

        /// <summary>
        /// The number of new feedback items for this work flow
        /// </summary>
        [DataMember]
        public int NewFeedbackInstanceCount { get; set; }

        /// <summary>
        /// Activity summary for this work flow item
        /// </summary>
        [DataMember]
        public ActivitySummaryDto[] ActivityItems { get; set; }
    }
}
