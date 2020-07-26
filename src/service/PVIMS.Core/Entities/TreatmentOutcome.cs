using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PVIMS.Core.Entities
{
	[Table(nameof(TreatmentOutcome))]
	public class TreatmentOutcome : EntityBase
	{
		[Required]
		[StringLength(50)]
		public string Description { get; set; }
	}
}