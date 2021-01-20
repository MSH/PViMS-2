using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PVIMS.Core.Entities
{
	[Table(nameof(EncounterType))]
	public class EncounterType : EntityBase
	{
		public EncounterType() 
		{
			Encounters = new HashSet<Encounter>();
			EncounterTypeWorkPlans = new HashSet<EncounterTypeWorkPlan>();
		}

		[Required]
		[StringLength(50)]
		public string Description { get; set; }

		[StringLength(250)]
		public string Help { get; set; }

        public bool Chronic { get; set; }

		public virtual ICollection<Encounter> Encounters { get; set; }
		public virtual ICollection<EncounterTypeWorkPlan> EncounterTypeWorkPlans { get; set; }
	}
}