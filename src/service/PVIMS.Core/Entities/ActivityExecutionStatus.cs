using System.ComponentModel.DataAnnotations.Schema;

namespace PVIMS.Core.Entities
{
    [Table(nameof(ActivityExecutionStatus))]
    public class ActivityExecutionStatus : EntityBase
	{
        public string Description { get; set; }
        public int ActivityId { get; set; }
        public string FriendlyDescription { get; set; }

        public virtual Activity Activity { get; set; }
    }
}