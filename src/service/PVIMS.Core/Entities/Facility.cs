using PVIMS.Core.Entities.Accounts;
using System.Collections.Generic;

namespace PVIMS.Core.Entities
{
    public class Facility : EntityBase
	{
		public Facility()
		{
			PatientFacilities = new HashSet<PatientFacility>();
            UserFacilities = new HashSet<UserFacility>();
		}

        public string FacilityCode { get; set; }
        public string FacilityName { get; set; }
        public int FacilityTypeId { get; set; }
        public string TelNumber { get; set; }
        public string MobileNumber { get; set; }
        public string FaxNumber { get; set; }
        public int? OrgUnitId { get; set; }

		public virtual FacilityType FacilityType { get; set; }
        public virtual OrgUnit OrgUnit { get; set; }

		public virtual ICollection<PatientFacility> PatientFacilities { get; set; }
        public virtual ICollection<UserFacility> UserFacilities { get; set; }

        public string DisplayName
        {
            get
            {
                return string.Format("{0} ({1})", FacilityName, FacilityCode);
            }
        }
    }
}