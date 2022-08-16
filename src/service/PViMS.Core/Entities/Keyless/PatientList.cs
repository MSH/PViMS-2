namespace PVIMS.Core.Entities.Keyless
{
    public class PatientList
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
