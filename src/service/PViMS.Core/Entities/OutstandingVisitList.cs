using System;

namespace PVIMS.Core.Entities
{
    public class OutstandingVisitList
    {
        public int Patient_Id { get; set; }
        public string FirstName { get; set; }
        public string Surname { get; set; }
        public DateTime AppointmentDate { get; set; }
    }
}
