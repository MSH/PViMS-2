using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PVIMS.Core.Entities
{
	[Table(nameof(ConceptIngredient))]
	public class ConceptIngredient : EntityBase
	{
		public ConceptIngredient()
		{
		}

		[Required]
		public virtual Concept Concept { get; set; }

		[Required]
		[StringLength(200)]
		public string Ingredient { get; set; }

		[Required]
		[StringLength(50)]
		public string Strength { get; set; }

		public bool Active { get; set; }
	}
}