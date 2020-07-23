using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PVIMS.Core.Entities
{
	[Table(nameof(Product))]
	public class Product : EntityBase
	{
		public Product()
		{
		}

		[Required]
		[StringLength(200)]
		public string ProductName { get; set; }

		[Required]
		[StringLength(200)]
		public string Manufacturer { get; set; }

		[Required]
		public virtual Concept Concept { get; set; }

		[StringLength(1000)]
		public string Description { get; set; }

		public bool Active { get; set; }
	}
}