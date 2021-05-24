using System.Collections.Generic;

namespace PVIMS.Core.Entities
{
	public class Concept : EntityBase
	{
		public Concept()
		{
			Active = true;

			ConceptIngredients = new HashSet<ConceptIngredient>();
			ConditionMedications = new HashSet<ConditionMedication>();
			PatientMedications = new HashSet<PatientMedication>();
			Products = new HashSet<Product>();
		}

		public string ConceptName { get; set; }
		public bool Active { get; set; }
		public int MedicationFormId { get; set; }

		public virtual MedicationForm MedicationForm { get; set; }

		public virtual ICollection<ConceptIngredient> ConceptIngredients { get; set; }
		public virtual ICollection<ConditionMedication> ConditionMedications { get; set; }
		public virtual ICollection<PatientMedication> PatientMedications { get; set; }
		public virtual ICollection<Product> Products { get; set; }
	}
}