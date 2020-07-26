using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PVIMS.Core.Entities
{
	[Table(nameof(TerminologyIcd10))]
	public class TerminologyIcd10 : EntityBase
	{
		[StringLength(20)]
		public string Name { get; set; }

		public string Description { get; set; }
	}
}