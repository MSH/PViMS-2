using PVIMS.Core.SeedWork;

namespace PVIMS.Core.Models
{
    public class DrugList : Entity<int>
    {
        public int ConceptId { get; set; }
        public string ConceptName { get; set; }
        public int PatientCount { get; set; }
    }
}
