using PVIMS.Core.SeedWork;

namespace PVIMS.Core.Models
{
    public class PatientList : Entity<int>
    {
        public int PatientId { get; set; }
        public string FirstName { get; set; }
        public string Surname { get; set; }
        public string FacilityName { get; set; }
        public string DateOfBirth { get; set; }
        public string Age { get; set; }
        public string LatestEncounterDate { get; set; }
    }
}
