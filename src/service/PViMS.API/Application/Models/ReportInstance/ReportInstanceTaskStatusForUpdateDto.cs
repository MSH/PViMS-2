using PVIMS.API.Infrastructure.Attributes;
using PVIMS.API.Models.ValueTypes;

namespace PVIMS.API.Models
{
    public class ReportInstanceTaskStatusForUpdateDto
    {
        /// <summary>
        /// The type of task
        /// </summary>
        [ValidEnumValue]
        public TaskStatusValueType TaskStatus { get; set; }
    }
}
