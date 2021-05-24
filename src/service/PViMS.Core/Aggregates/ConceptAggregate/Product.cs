using System.Collections.Generic;

namespace PVIMS.Core.Entities
{
	public class Product : EntityBase
	{
		public Product()
		{
			ConditionMedications = new HashSet<ConditionMedication>();
			PatientMedications = new HashSet<PatientMedication>();
		}

		public string ProductName { get; set; }
		public string Manufacturer { get; set; }
		public string Description { get; set; }
		public bool Active { get; set; }
		public int ConceptId { get; set; }

		public virtual Concept Concept { get; set; }

		public virtual ICollection<ConditionMedication> ConditionMedications { get; set; }
		public virtual ICollection<PatientMedication> PatientMedications { get; set; }
	}
}