using PVIMS.Core.Aggregates.ReportInstanceAggregate;
using System.ComponentModel.DataAnnotations;

namespace PVIMS.API.Models
{
    public class ReportInstanceTaskForCreationDto
    {
        /// <summary>
        /// The source of that data quality task
        /// </summary>
        [Required]
        public string Source { get; private set; }

        /// <summary>
        /// A detailed description of the task requirement
        /// </summary>
        [Required]
        public string Description { get; private set; }

        /// <summary>
        /// The type of task
        /// </summary>
        [Required]
        public TaskType TaskType { get; private set; }
    }
}
