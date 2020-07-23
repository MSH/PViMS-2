using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PVIMS.Core.Entities
{
	[Table(nameof(Concept))]
	public class Concept : EntityBase
	{
		public Concept()
		{
			ConceptIngredients = new HashSet<ConceptIngredient>();
			Products = new HashSet<Product>();
		}

		[Required]
		[StringLength(1000)]
		public string ConceptName { get; set; }

		[Required]
		public virtual MedicationForm MedicationForm { get; set; }

		public bool Active { get; set; }

		public virtual ICollection<ConceptIngredient> ConceptIngredients { get; set; }
		public virtual ICollection<Product> Products { get; set; }
	}
}