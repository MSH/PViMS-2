using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PVIMS.Core.Entities
{
	[Table(nameof(PatientStatus))]
	public class PatientStatus : EntityBase
	{
		public PatientStatus()
		{
			PatientStatusHistories = new HashSet<PatientStatusHistory>();
		}

		[Required]
		[StringLength(50)]
		public string Description { get; set; }

		public virtual ICollection<PatientStatusHistory> PatientStatusHistories { get; set; }
	}
}