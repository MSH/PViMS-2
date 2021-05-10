using PVIMS.API.Infrastructure.Attributes;
using PVIMS.API.Models.ValueTypes;

namespace PVIMS.API.Models
{
    public class ReportInstanceTaskForCreationDto
    {
        /// <summary>
        /// The source of that data quality task
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        /// A detailed description of the task requirement
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The type of task
        /// </summary>
        [ValidEnumValue]
        public TaskTypeValueType TaskType { get; set; }
    }
}
