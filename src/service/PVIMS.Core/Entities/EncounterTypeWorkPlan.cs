using System.ComponentModel.DataAnnotations.Schema;

namespace PVIMS.Core.Entities
{
	[Table(nameof(EncounterTypeWorkPlan))]
	public class EncounterTypeWorkPlan : EntityBase
	{
		public virtual CohortGroup CohortGroup { get; set; }
		public virtual EncounterType EncounterType { get; set; }
		public virtual WorkPlan WorkPlan { get; set; }
	}
}