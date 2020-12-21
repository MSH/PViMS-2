namespace PVIMS.Core.Entities
{
    public class AppointmentList
    {
        public int AppointmentId { get; set; }
        public int PatientId { get; set; }
        public int EncounterId { get; set; }
        public string FirstName { get; set; }
        public string Surname { get; set; }
        public string FacilityName { get; set; }
        public string AppointmentDate { get; set; }
        public string Reason { get; set; }
        public string AppointmentStatus { get; set; }
    }
}
