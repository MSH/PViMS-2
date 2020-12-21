using PVIMS.API.Infrastructure.Attributes;
using PVIMS.Core.ValueTypes;

namespace PVIMS.API.Models.Parameters
{
    public class AdverseEventReportResourceParameters : BaseReportResourceParameters
    {
        /// <summary>
        /// Filter reports by criteria
        /// </summary>
        [ValidEnumValue]
        public AdverseEventCriteria AdverseEventCriteria { get; set; } = AdverseEventCriteria.ReportSource;

        /// <summary>
        /// Filter reports by criteria
        /// </summary>
        [ValidEnumValue]
        public AdverseEventStratifyCriteria AdverseEventStratifyCriteria { get; set; } = AdverseEventStratifyCriteria.AgeGroup;
    }
}
