using System.ComponentModel.DataAnnotations;

namespace PVIMS.API.Models
{
    public class ActivityForUpdateDto
    {
        /// <summary>
        /// The status the report instance should be set too
        /// </summary>
        [Required]
        [StringLength(50)]
        public string NewExecutionStatus { get; set; }

        /// <summary>
        /// Comments associated to the change in status
        /// </summary>
        [StringLength(100)]
        public string Comments { get; set; }
    }
}
