using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PVIMS.Core.Entities
{
	[Table(nameof(FacilityType))]
	public class FacilityType : EntityBase
	{
		public FacilityType()
		{
			Facilities = new HashSet<Facility>();
		}

		[Required]
		[StringLength(50)]
		public string Description { get; set; }

		public virtual ICollection<Facility> Facilities { get; set; }
	}
}