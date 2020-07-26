using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PVIMS.Core.Entities
{
	[Table(nameof(ConditionMedication))]
	public class ConditionMedication : EntityBase
	{
		[Required]
		public virtual Condition Condition { get; set; }

		public virtual Concept Concept { get; set; }
		public virtual Product Product { get; set; }
	}
}