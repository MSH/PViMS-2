namespace PVIMS.Core.Entities
{
    public class EncounterList
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
