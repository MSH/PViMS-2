using System;

namespace PVIMS.API.Models.Parameters
{
    public class AnalyserTermSetResourceParameters : BaseResourceParameters
    {
        /// <summary>
        /// Filter analysis by selected condition
        /// </summary>
        public int ConditionId { get; set; }

        /// <summary>
        /// Filter analysis by selected cohort group
        /// </summary>
        public int CohortGroupId { get; set; }

        /// <summary>
        /// Filter by range
        /// </summary>
        public DateTime SearchFrom { get; set; } = DateTime.MinValue;

        /// <summary>
        /// Filter by range
        /// </summary>
        public DateTime SearchTo { get; set; } = DateTime.MaxValue;
    }
}
