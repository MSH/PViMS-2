using System;

using VPS.Common.Domain;

namespace PVIMS.Core.Models
{
    public class CausalityNotSetList : Entity<int>
    {
        public int Patient_Id { get; set; }
        public string FacilityName { get; set; }
        public string FirstName { get; set; }
        public string Surname { get; set; }
        public string AdverseEvent { get; set; }
        public string Serious { get; set; }
        public DateTime OnsetDate { get; set; }
        public string NaranjoCausality { get; set; }
        public string WhoCausality { get; set; }
        public string Medicationidentifier { get; set; }
    }
}
