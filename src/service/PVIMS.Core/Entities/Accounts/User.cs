using System;
using System.Collections.Generic;
using System.Linq;

namespace PVIMS.Core.Entities.Accounts
{
    public class User : EntityBase
	{
        public User()
		{
            ActivityInstanceCreations = new HashSet<ActivityInstance>();
            ActivityInstanceUpdates = new HashSet<ActivityInstance>();
            Appointments = new HashSet<Appointment>();
            AppointmentCreations = new HashSet<Appointment>();
            AppointmentUpdates = new HashSet<Appointment>();
            Attachments = new HashSet<Attachment>();
            AttachmentCreations = new HashSet<Attachment>();
            AttachmentUpdates = new HashSet<Attachment>();
            AuditLogs = new HashSet<AuditLog>();
            CohortGroupEnrolments = new HashSet<CohortGroupEnrolment>();
            ConfigCreations = new HashSet<Config>();
            ConfigUpdates = new HashSet<Config>();
            DatasetCreations = new HashSet<Dataset>();
            DatasetUpdates = new HashSet<Dataset>();
            DatasetInstanceCreations = new HashSet<DatasetInstance>();
            DatasetInstanceUpdates = new HashSet<DatasetInstance>();
            DatasetXmlCreations = new HashSet<DatasetXml>();
            DatasetXmlUpdates = new HashSet<DatasetXml>();
            DatasetXmlAttributeCreations = new HashSet<DatasetXmlAttribute>();
            DatasetXmlAttributeUpdates = new HashSet<DatasetXmlAttribute>();
            DatasetXmlNodeCreations = new HashSet<DatasetXmlNode>();
            DatasetXmlNodeUpdates = new HashSet<DatasetXmlNode>();
            Encounters = new HashSet<Encounter>();
            EncounterCreations = new HashSet<Encounter>();
            EncounterUpdates = new HashSet<Encounter>();
            ExecutionEvents = new HashSet<ActivityExecutionStatusEvent>();
            Facilities = new HashSet<UserFacility>();
            PatientCreations = new HashSet<Patient>();
            PatientUpdates = new HashSet<Patient>();
            PatientClinicalEvents = new HashSet<PatientClinicalEvent>();
            PatientConditions = new HashSet<PatientCondition>();
            PatientFacilities = new HashSet<PatientFacility>();
            PatientLabTests = new HashSet<PatientLabTest>();
            PatientMedications = new HashSet<PatientMedication>();
            PatientStatusHistories = new HashSet<PatientStatusHistory>();
            PatientStatusHistoryCreations = new HashSet<PatientStatusHistory>();
            PatientStatusHistoryUpdates = new HashSet<PatientStatusHistory>();
            Patients = new HashSet<Patient>();
            PregnancyCreations = new HashSet<Pregnancy>();
            PregnancyUpdates = new HashSet<Pregnancy>();
            RefreshTokens = new HashSet<RefreshToken>();
            ReportInstanceCreations = new HashSet<ReportInstance>();
            ReportInstanceUpdates = new HashSet<ReportInstance>();
            SiteContactDetailCreations = new HashSet<SiteContactDetail>();
            SiteContactDetailUpdates = new HashSet<SiteContactDetail>();
            SystemLogCreations = new HashSet<SystemLog>();
            SystemLogUpdates = new HashSet<SystemLog>();

            AllowDatasetDownload = false;
        }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public DateTime? EulaAcceptanceDate { get; set; }
        public bool AllowDatasetDownload { get; set; }
        public Guid IdentityId { get; set; }
        public UserType UserType { get; set; }

        public virtual ICollection<ActivityInstance> ActivityInstanceCreations { get; set; }
        public virtual ICollection<ActivityInstance> ActivityInstanceUpdates { get; set; }
        public virtual ICollection<Appointment> Appointments { get; set; }
        public virtual ICollection<Appointment> AppointmentCreations { get; set; }
        public virtual ICollection<Appointment> AppointmentUpdates { get; set; }
        public virtual ICollection<Attachment> Attachments { get; set; }
        public virtual ICollection<Attachment> AttachmentCreations { get; set; }
        public virtual ICollection<Attachment> AttachmentUpdates { get; set; }
        public virtual ICollection<AuditLog> AuditLogs { get; set; }
        public virtual ICollection<CohortGroupEnrolment> CohortGroupEnrolments { get; set; }
        public virtual ICollection<Config> ConfigCreations { get; set; }
        public virtual ICollection<Config> ConfigUpdates { get; set; }
        public virtual ICollection<Dataset> DatasetCreations { get; set; }
        public virtual ICollection<Dataset> DatasetUpdates { get; set; }
        public virtual ICollection<DatasetInstance> DatasetInstanceCreations { get; set; }
        public virtual ICollection<DatasetInstance> DatasetInstanceUpdates { get; set; }
        public virtual ICollection<DatasetXml> DatasetXmlCreations { get; set; }
        public virtual ICollection<DatasetXml> DatasetXmlUpdates { get; set; }
        public virtual ICollection<DatasetXmlAttribute> DatasetXmlAttributeCreations { get; set; }
        public virtual ICollection<DatasetXmlAttribute> DatasetXmlAttributeUpdates { get; set; }
        public virtual ICollection<DatasetXmlNode> DatasetXmlNodeCreations { get; set; }
        public virtual ICollection<DatasetXmlNode> DatasetXmlNodeUpdates { get; set; }
        public virtual ICollection<Encounter> Encounters { get; set; }
        public virtual ICollection<Encounter> EncounterCreations { get; set; }
        public virtual ICollection<Encounter> EncounterUpdates { get; set; }
        public virtual ICollection<ActivityExecutionStatusEvent> ExecutionEvents { get; set; }
        public virtual ICollection<UserFacility> Facilities { get; set; }
        public virtual ICollection<Patient> PatientCreations { get; set; }
        public virtual ICollection<Patient> PatientUpdates { get; set; }
        public virtual ICollection<PatientClinicalEvent> PatientClinicalEvents { get; set; }
        public virtual ICollection<PatientCondition> PatientConditions { get; set; }
        public virtual ICollection<PatientFacility> PatientFacilities { get; set; }
        public virtual ICollection<PatientLabTest> PatientLabTests { get; set; }
        public virtual ICollection<PatientMedication> PatientMedications { get; set; }
        public virtual ICollection<PatientStatusHistory> PatientStatusHistories { get; set; }
        public virtual ICollection<PatientStatusHistory> PatientStatusHistoryCreations { get; set; }
        public virtual ICollection<PatientStatusHistory> PatientStatusHistoryUpdates { get; set; }
        public virtual ICollection<Patient> Patients { get; set; }
        public virtual ICollection<Pregnancy> PregnancyCreations { get; set; }
        public virtual ICollection<Pregnancy> PregnancyUpdates { get; set; }
        public virtual ICollection<RefreshToken> RefreshTokens { get; set; }
        public virtual ICollection<ReportInstance> ReportInstanceCreations { get; set; }
        public virtual ICollection<ReportInstance> ReportInstanceUpdates { get; set; }
        public virtual ICollection<SiteContactDetail> SiteContactDetailCreations { get; set; }
        public virtual ICollection<SiteContactDetail> SiteContactDetailUpdates { get; set; }
        public virtual ICollection<SystemLog> SystemLogCreations { get; set; }
        public virtual ICollection<SystemLog> SystemLogUpdates { get; set; }

        public string FullName
        {
            get { return string.Format("{0} {1}", FirstName, LastName); }
        }

        public bool HasFacility(int id) =>
            Facilities.Any(uf => uf.Facility.Id == id);

        //public bool HasRole(string[] roles) =>
        //    Roles.Any(ur => roles.Contains(ur.Role?.Key));

        public bool HasValidRefreshToken(string refreshToken) =>
            RefreshTokens.Any(rt => rt.Token == refreshToken && rt.Active);

        public void AddRefreshToken(string token, string remoteIpAddress, double daysToExpire = 7) =>
            RefreshTokens.Add(new RefreshToken(token, DateTime.UtcNow.AddDays(daysToExpire), remoteIpAddress));

        public void RemoveRefreshToken(string refreshToken) =>  
            RefreshTokens.Remove(RefreshTokens.First(t => t.Token == refreshToken));
    }
}