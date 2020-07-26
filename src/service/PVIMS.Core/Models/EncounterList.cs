using VPS.Common.Domain;

namespace PVIMS.Core.Models
{
    public class EncounterList : Entity<int>
    {
        public int EncounterId { get; set; }
        public int PatientId { get; set; }
        public string FirstName { get; set; }
        public string Surname { get; set; }
        public string FacilityName { get; set; }
        public string EncounterType { get; set; }
        public string EncounterDate { get; set; }
    }
}
