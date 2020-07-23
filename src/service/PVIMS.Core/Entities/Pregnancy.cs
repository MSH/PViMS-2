using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PVIMS.Core.Entities
{
	[Table(nameof(Pregnancy))]
	public class Pregnancy : AuditedEntityBase
	{
		public Pregnancy()
		{
			Encounters = new HashSet<Encounter>();
		}

		public virtual Patient Patient { get; set; }
		public DateTime StartDate { get; set; }
		public DateTime? FinishDate { get; set; }

		[Required]
		[StringLength(10)]
		public string PreferredFeedingChoice { get; set; }

		public short? InitialGestation { get; set; }
		public DateTime? ExpectedDeliveryDate { get; set; }
		public DateTime? ActualDeliveryDate { get; set; }
		public virtual ICollection<Encounter> Encounters { get; set; }
	}
}