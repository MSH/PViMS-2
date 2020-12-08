using PVIMS.Core.SeedWork;
using System;

namespace PVIMS.Core.Models
{
    public class OutstandingVisitList : Entity<int>
    {
        public int Patient_Id { get; set; }
        public string FirstName { get; set; }
        public string Surname { get; set; }
        public DateTime AppointmentDate { get; set; }
    }
}
