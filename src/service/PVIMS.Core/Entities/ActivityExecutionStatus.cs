using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PVIMS.Core.Entities
{
    [Table(nameof(ActivityExecutionStatus))]
    public class ActivityExecutionStatus : EntityBase
	{
        [Required]
        public virtual Activity Activity { get; set; }

        [Required]
		[StringLength(50)]
		public string Description { get; set; }

        [StringLength(100)]
        public string FriendlyDescription { get; set; }

    }
}