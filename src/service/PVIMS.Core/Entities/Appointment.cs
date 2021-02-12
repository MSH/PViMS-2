using PVIMS.Core.Entities.Accounts;
using System;
using System.Linq;

namespace PVIMS.Core.Entities
{
    public class Appointment : AuditedEntityBase
    {
        protected Appointment() { }

        public Appointment(Patient patient)
        {
            Patient = patient;
        }

        public DateTime AppointmentDate { get; set; }
        public string Reason { get; set; }
        public bool Dna { get; set; }
        public bool Cancelled { get; set; }
        public string CancellationReason { get; set; }
        public int PatientId { get; set; }
        public bool Archived { get; set; }
        public DateTime? ArchivedDate { get; set; }
        public string ArchivedReason { get; set; }
        public int? AuditUserId { get; set; }

        public virtual User AuditUser { get; set; }
        public virtual Patient Patient { get; set; }

        public Encounter GetEncounter()
        {
            return Patient.Encounters
                .Where(e => (e.EncounterDate >= AppointmentDate.AddDays(1) && e.EncounterDate <= AppointmentDate.AddDays(5)))
                .FirstOrDefault();
        }
    }
}
