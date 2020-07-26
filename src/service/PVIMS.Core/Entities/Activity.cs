using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PVIMS.Core.ValueTypes;

namespace PVIMS.Core.Entities
{
    [Table(nameof(Activity))]
    public class Activity : EntityBase
	{
        public Activity()
        {
            ExecutionStatuses = new HashSet<ActivityExecutionStatus>();
        }

        [Required]
        public virtual WorkFlow WorkFlow { get; set; }

        [Required]
		[StringLength(50)]
		public string QualifiedName { get; set; }

        [Required]
        public virtual ActivityTypes ActivityType { get; set; }

        public virtual ICollection<ActivityExecutionStatus> ExecutionStatuses { get; set; }
    }
}