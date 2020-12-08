using PVIMS.Core.SeedWork;

namespace PVIMS.Core.Models
{
    public class AdverseEventList : Entity<int>
    {
        public string Description { get; set; }
        public string Criteria { get; set; }
        public string Serious { get; set; }
        public int PatientCount { get; set; }
    }
}
