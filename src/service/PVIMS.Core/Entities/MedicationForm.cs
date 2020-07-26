using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PVIMS.Core.Entities
{
	[Table(nameof(MedicationForm))]
	public class MedicationForm : EntityBase
	{
		public MedicationForm()
		{
		}

		[Required]
		[StringLength(50)]
		public string Description { get; set; }
	}
}