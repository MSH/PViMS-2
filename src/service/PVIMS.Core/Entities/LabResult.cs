using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PVIMS.Core.Entities
{
	[Table(nameof(LabResult))]
	public class LabResult : EntityBase
	{
		[Required]
		[StringLength(50)]
		public string Description { get; set; }

		public bool Active { get; set; }
	}
}