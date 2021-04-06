namespace PVIMS.Core.Entities
{
	public class ConceptIngredient : EntityBase
	{
		public ConceptIngredient()
		{
			Active = true;
		}

		public string Ingredient { get; set; }
		public string Strength { get; set; }
		public bool Active { get; set; }
		public int ConceptId { get; set; }

		public virtual Concept Concept { get; set; }
	}
}