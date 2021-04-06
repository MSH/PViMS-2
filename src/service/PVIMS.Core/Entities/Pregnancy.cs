using System;
using System.Collections.Generic;

namespace PVIMS.Core.Entities
{
	public class Pregnancy : AuditedEntityBase
	{
		public Pregnancy()
		{
			Encounters = new HashSet<Encounter>();
		}

		public DateTime StartDate { get; set; }
		public DateTime? FinishDate { get; set; }
		public string PreferredFeedingChoice { get; set; }
		public short? InitialGestation { get; set; }
		public DateTime? ExpectedDeliveryDate { get; set; }
		public DateTime? ActualDeliveryDate { get; set; }
		public int PatientId { get; set; }

		public virtual Patient Patient { get; set; }

		public virtual ICollection<Encounter> Encounters { get; set; }
	}
}