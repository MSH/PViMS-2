using PVIMS.Core.CustomAttributes;
using PVIMS.Core.Entities.Accounts;
using PVIMS.Core.Exceptions;
using PVIMS.Core.Models;
using PVIMS.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PVIMS.Core.Entities
{
    public class Patient : AuditedEntityBase, IExtendable
	{
		public Patient()
		{
            Appointments = new HashSet<Appointment>();
            Attachments = new HashSet<Attachment>();
            CohortEnrolments = new HashSet<CohortGroupEnrolment>();
            Encounters = new HashSet<Encounter>();
			PatientClinicalEvents = new HashSet<PatientClinicalEvent>();
			PatientConditions = new HashSet<PatientCondition>();
			PatientFacilities = new HashSet<PatientFacility>();
			PatientLabTests = new HashSet<PatientLabTest>();
			PatientLanguages = new HashSet<PatientLanguage>();
			PatientMedications = new HashSet<PatientMedication>();
			PatientStatusHistories = new HashSet<PatientStatusHistory>();

            PatientGuid = Guid.NewGuid();
        }

        public DateTime? DateOfBirth { get; set; }
        public string FirstName { get; set; }
        public string Surname { get; set; }
        public string Notes { get; set; }
        public Guid PatientGuid { get; set; }
        public string MiddleName { get; set; }
        public bool Archived { get; set; }
        public DateTime? ArchivedDate { get; set; }
        public string ArchivedReason { get; set; }
        public int? AuditUserId { get; set; }

        public virtual User AuditUser { get; set; }

        public virtual ICollection<Appointment> Appointments { get; set; }
        public virtual ICollection<Attachment> Attachments { get; set; }
        public virtual ICollection<CohortGroupEnrolment> CohortEnrolments { get; set; }
        public virtual ICollection<Encounter> Encounters { get; set; }
		public virtual ICollection<PatientClinicalEvent> PatientClinicalEvents { get; set; }
		public virtual ICollection<PatientCondition> PatientConditions { get; set; }
		public virtual ICollection<PatientFacility> PatientFacilities { get; set; }
		public virtual ICollection<PatientLabTest> PatientLabTests { get; set; }
		public virtual ICollection<PatientLanguage> PatientLanguages { get; set; }
		public virtual ICollection<PatientMedication> PatientMedications { get; set; }
		public virtual ICollection<PatientStatusHistory> PatientStatusHistories { get; set; }

        public int Age
        {
            get
            {
                if (DateOfBirth == null) {
                    return 0;
                }

                DateTime today = DateTime.Today;
                int age = today.Year - Convert.ToDateTime(DateOfBirth).Year;
                if (Convert.ToDateTime(DateOfBirth) > today.AddYears(-age)) age--;

                return age;
            }
        }

        public string AgeGroup
        {
            get
            {
                if (DateOfBirth == null)
                {
                    return "";
                }

                DateTime today = DateTime.Today;
                DateTime bday = Convert.ToDateTime(DateOfBirth);

                string ageGroup = "";
                if (today <= bday.AddMonths(1)) { ageGroup = "Neonate <= 1 month"; };
                if (today <= bday.AddMonths(48) && today > bday.AddMonths(1)) { ageGroup = "Infant > 1 month and <= 4 years"; };
                if (today <= bday.AddMonths(132) && today > bday.AddMonths(48)) { ageGroup = "Child > 4 years and <= 11 years"; };
                if (today <= bday.AddMonths(192) && today > bday.AddMonths(132)) { ageGroup = "Adolescent > 11 years and <= 16 years"; };
                if (today <= bday.AddMonths(828) && today > bday.AddMonths(192)) { ageGroup = "Adult > 16 years and <= 69 years"; };
                if (today > bday.AddMonths(828)) { ageGroup = "Elderly > 69 years"; };

                return ageGroup;
            }
        }

        public string FullName
        {
            get
            {
                return FirstName.Trim() + ' ' + Surname.Trim();
            }
        }

        public string CurrentFacilityName
        {
            get
            {
                var currentFacility = GetCurrentFacility();

                return currentFacility == null ? "** Not set **" : currentFacility.Facility.FacilityName;
            }
        }

        public DateTime? LatestEncounterDate
        {
            get
            {
                if (Encounters.Count == 0)
                {
                    return null;
                }
                else
                {
                    return Encounters.OrderByDescending(e => e.EncounterDate).FirstOrDefault().EncounterDate;
                }
            }
        }

        public Encounter GetEncounterForAppointment(Appointment app)
        {
            if (Encounters.Count == 0 || app == null) {
                return null;
            }
            else {
                return Encounters.FirstOrDefault(e => e.EncounterDate >= app.AppointmentDate.AddDays(-3) && e.EncounterDate <= app.AppointmentDate.AddDays(3));
            }
        }

        public PatientFacility GetCurrentFacility()
        {
            if (PatientFacilities.Count == 0) {
                return null;
            }
            else {
                return PatientFacilities.OrderByDescending(f => f.EnrolledDate).ThenByDescending(f => f.Id).First();
            }
        }

        public Encounter GetCurrentEncounter()
        {
            if (Encounters.Count == 0) {
                return null;
            }
            else
            {
                return Encounters.OrderByDescending(e => e.EncounterDate).ThenByDescending(e => e.Id).First();
            }
        }

        public PatientStatusHistory GetCurrentStatus()
        {
            if (PatientStatusHistories.Count == 0)
            {
                return null;
            }
            else
            {
                return PatientStatusHistories.OrderByDescending(psh => psh.EffectiveDate).ThenByDescending(psh => psh.Id).First();
            }
        }

        public Encounter GetEncounterForToday()
        {
            if (Encounters.Count == 0) {
                return null;
            }
            else
            {
                return Encounters.SingleOrDefault(e => e.EncounterDate == DateTime.Today);
            }
        }

        private CustomAttributeSet customAttributes = new CustomAttributeSet();

        CustomAttributeSet IExtendable.CustomAttributes
        {
            get { return customAttributes; }
        }

        public string CustomAttributesXmlSerialised
        {

            get { return SerialisationHelper.SerialiseToXmlString(customAttributes); }
            private set
            {
                if (string.IsNullOrEmpty(value))
                {
                    customAttributes = new CustomAttributeSet();
                }
                else
                {
                    customAttributes = SerialisationHelper.DeserialiseFromXmlString<CustomAttributeSet>(value);
                }
            }
        }

        void IExtendable.SetAttributeValue<T>(string attributeKey, T attributeValue, string updatedByUser)
        {
            customAttributes.SetAttributeValue(attributeKey, attributeValue, updatedByUser);
        }

        object IExtendable.GetAttributeValue(string attributeKey)
        {
            return customAttributes.GetAttributeValue(attributeKey);
        }

        public void ValidateAndSetAttributeValue<T>(CustomAttributeConfiguration attributeConfig, T attributeValue, string updatedByUser)
        {
            customAttributes.ValidateAndSetAttributeValue(attributeConfig, attributeValue, updatedByUser);
        }

        public DateTime GetUpdatedDate(string attributeKey)
        {
            return customAttributes.GetUpdatedDate(attributeKey);
        }

        public string GetUpdatedByUser(string attributeKey)
        {
            return customAttributes.GetUpdatedByUser(attributeKey);
        }

        public bool HasAppointment(int id, DateTime apptDate)
        {
            if (Appointments.Count == 0) {
                return false;
            }
            else 
            {
                if(id > 0) {
                    return Appointments.Any(a => a.AppointmentDate == apptDate && a.Id != id);
                }
                else {
                    return Appointments.Any(a => a.AppointmentDate == apptDate);
                }
            }
        }

        public CohortGroupEnrolment GetCohortEnrolled(CohortGroup cohort)
        {
            if (CohortEnrolments.Count == 0) {
                return null;
            }
            else {
                return CohortEnrolments.FirstOrDefault(ce => ce.CohortGroup.Id == cohort.Id && !ce.Archived );
            }
        }

        public PatientCondition AddOrUpdatePatientCondition(long id,
            TerminologyMedDra sourceTerm, 
            DateTime onsetDate, 
            DateTime? outComeDate, 
            Outcome outcome, 
            TreatmentOutcome treatmentOutcome, 
            string comments,
            string conditionSource,
            PatientStatus deceasedStatus)
        {
            PatientCondition patientCondition = null;

            if(id == 0)
            {
                patientCondition = new PatientCondition
                {
                    ConditionSource = conditionSource,
                    TerminologyMedDra = sourceTerm,
                    OnsetDate = onsetDate,
                    OutcomeDate = outComeDate,
                    Outcome = outcome,
                    TreatmentOutcome = treatmentOutcome,
                    Comments = comments
                };

                PatientConditions.Add(patientCondition);
            }

            // Has person died
            if (outcome?.Description == "Fatal" && GetCurrentStatus()?.PatientStatus.Description != "Died")
            {
                // set patient status to deceased in patient history
                PatientStatusHistories.Add(new PatientStatusHistory()
                {
                    EffectiveDate = outComeDate ?? DateTime.Now,   //set effective date to  outcome date have set it to  use todays day if null but this will not happen as  autosetToDeceased will only become true when an end date is supplied first
                    Comments = $"Marked as deceased through Patient Condition ({sourceTerm.DisplayName})",
                    PatientStatus = deceasedStatus
                });
            }

            return patientCondition;
        }

        public bool HasCondition(List<Condition> conditions)
        {
            if (PatientConditions.Count == 0)
            {
                return false;
            }
            else
            {
                foreach (var pc in PatientConditions)
                {
                    foreach (var termcond in pc.TerminologyMedDra.ConditionMedDras)
                    {
                        // Go and check each condition this terminology is tied to
                        if (conditions.Contains(termcond.Condition)) {
                            return true;
                        }

                    }

                }
                return false;
            }
        }

        public PatientCondition GetConditionForGroupAndDate(string condition, DateTime date)
        {
            if (PatientConditions.Count == 0)
            {
                return null;
            }
            else
            {
                return PatientConditions.Where(pc => pc.Archived == false && pc.TerminologyMedDra != null)
                    .Where(pc => pc.OnsetDate <= date 
                        && pc.TerminologyMedDra.ConditionMedDras.Any(cm => cm.Condition.Description == condition))
                    .OrderByDescending(pc => pc.OnsetDate)
                    .FirstOrDefault();
            }
        }

        public bool HasClinicalData()
        {
            var hasData = false;

            if (PatientClinicalEvents.Count() > 0 || PatientConditions.Count() > 0 || PatientLabTests.Count() > 0 || PatientMedications.Count() > 0 || Encounters.Count() > 0) { hasData = true; };

            return hasData;
        }

        public PatientEventSummary GetEventSummary()
        {
            var seriesCount = 0;
            var nonSeriesCount = 0;

            IExtendable clinicalEventExtended;

            foreach (PatientClinicalEvent clinicalEvent in PatientClinicalEvents.Where(pce => pce.Archived == false))
            {
                clinicalEventExtended = clinicalEvent;
                var value = clinicalEventExtended.GetAttributeValue("Is the adverse event serious?").ToString();
                if(value == "1")
                {
                    seriesCount += 1;
                }
                else
                {
                    nonSeriesCount += 1;
                }
            }

            var model = new PatientEventSummary()
            {
                PatientId = Id,
                NonSeriesEventCount = nonSeriesCount,
                SeriesEventCount = seriesCount
            };
            return model;
        }

        public void SetPatientFacility(Facility facility)
        {
            if (facility == null)
            {
                throw new ArgumentNullException(nameof(facility));
            }

            // Exit if current facility is still the same
            var currentFacility = GetCurrentFacility();
            if(currentFacility?.Facility.Id == facility.Id) { return; };

            // Link patient to new facility
            var patientFacility = new PatientFacility 
            {
                Patient = this,
                Facility = facility, 
                EnrolledDate = DateTime.Today 
            };
            this.PatientFacilities.Add(patientFacility);
        }

        public void SetPatientStatus(PatientStatus patientStatus)
        {
            var patientStatusHistory = new PatientStatusHistory()
            {
                Patient = this,
                EffectiveDate = DateTime.Today,
                Comments = "New Patient",
                PatientStatus = patientStatus
            };
            this.PatientStatusHistories.Add(patientStatusHistory);
        }

        public PatientClinicalEvent AddClinicalEvent(DateTime? onsetDate, DateTime? resolutionDate, TerminologyMedDra sourceTerminology, string sourceDescription)
        {
            if (onsetDate < DateOfBirth)
            {
                throw new DomainException("Onset Date should be after Date Of Birth");
            }

            if (resolutionDate.HasValue)
            {
                if (resolutionDate < DateOfBirth)
                {
                    throw new DomainException("Resolution Date should be after Date Of Birth");
                }
            }

            if(sourceTerminology != null && onsetDate.HasValue)
            {
                if (CheckEventOnsetDateAgainstOnsetDateWithNoResolutionDate(sourceTerminology.Id, onsetDate.Value, 0))
                {
                    throw new DomainException("Duplication of adverse event. Please check onset and resolution dates");
                }
                else
                {
                    if (CheckEventOnsetDateWithinRange(sourceTerminology.Id, onsetDate.Value, 0))
                    {
                        throw new DomainException("Duplication of adverse event. Please check onset and resolution dates");
                    }
                    else
                    {
                        if (resolutionDate.HasValue)
                        {
                            if (CheckEventOnsetDateWithNoResolutionDateBeforeOnset(sourceTerminology.Id, onsetDate.Value, 0))
                            {
                                throw new DomainException("Duplication of adverse event. Please check onset and resolution dates");
                            }
                        }
                    }
                }

                // Check clinical event overlapping - RESOLUTION DATE
                if (resolutionDate.HasValue)
                {
                    if (CheckEventResolutionDateAgainstOnsetDateWithNoResolutionDate(sourceTerminology.Id, resolutionDate.Value, 0))
                    {
                        throw new DomainException("Duplication of adverse event. Please check onset and resolution dates");
                    }
                    else
                    {
                        if (CheckEventResolutionDateWithinRange(sourceTerminology.Id, resolutionDate.Value, 0))
                        {
                            throw new DomainException("Duplication of adverse event. Please check onset and resolution dates");
                        }
                    }
                }
            }

            var newPatientClinicalEvent = new PatientClinicalEvent(onsetDate, resolutionDate, sourceTerminology, sourceDescription);
            PatientClinicalEvents.Add(newPatientClinicalEvent);
            return newPatientClinicalEvent;
        }

        public PatientMedication AddMedication(Concept concept, DateTime startDate, DateTime? endDate, string dose, string doseFrequency, string doseUnit, Product product, string medicationSource)
        {
            if (startDate < DateOfBirth)
            {
                throw new DomainException("Start Date should be after Date Of Birth");
            }

            if (endDate.HasValue)
            {
                if (endDate < DateOfBirth)
                {
                    throw new DomainException("End Date should be after Date Of Birth");
                }
            }

            if (CheckMedicationStartDateAgainstStartDateWithNoEndDate(concept.Id, startDate, 0))
            {
                throw new DomainException("Duplication of medication. Please check start and end dates");
            }
            else
            {
                if (CheckMedicationStartDateWithinRange(concept.Id, startDate, 0))
                {
                    throw new DomainException("Duplication of medication. Please check start and end dates");
                }
                else
                {
                    if (endDate.HasValue)
                    {
                        if (CheckMedicationStartDateWithNoEndDateBeforeStart(concept.Id, startDate, 0))
                        {
                            throw new DomainException("Duplication of medication. Please check start and end dates");
                        }
                    }
                }
            }

            if (endDate.HasValue)
            {
                if (CheckMedicationEndDateAgainstStartDateWithNoEndDate(concept.Id, endDate.Value, 0))
                {
                    throw new DomainException("Duplication of medication. Please check start and end dates");
                }
                else
                {
                    if (CheckMedicationEndDateWithinRange(concept.Id, endDate.Value, 0))
                    {
                        throw new DomainException("Duplication of medication. Please check start and end dates");
                    }
                }
            }

            var newPatientMedication = new PatientMedication(concept, startDate, endDate, dose, doseFrequency, doseUnit, product, medicationSource);
            PatientMedications.Add(newPatientMedication);
            return newPatientMedication;
        }

        public void ChangeClinicalEventDetails(int patientClinicalEventId, DateTime? onsetDate, DateTime? resolutionDate, TerminologyMedDra sourceTerminology, string sourceDescription)
        {
            var patientClinicalEvent = PatientClinicalEvents.SingleOrDefault(t => t.Id == patientClinicalEventId);
            if (patientClinicalEvent == null)
            {
                throw new KeyNotFoundException(nameof(patientClinicalEventId));
            }

            if (onsetDate < DateOfBirth)
            {
                throw new DomainException("Onset Date should be after Date Of Birth");
            }

            if (resolutionDate.HasValue)
            {
                if (resolutionDate < DateOfBirth)
                {
                    throw new DomainException("Resolution Date should be after Date Of Birth");
                }
            }

            if (sourceTerminology != null && onsetDate.HasValue)
            {
                if (CheckEventOnsetDateAgainstOnsetDateWithNoResolutionDate(sourceTerminology.Id, onsetDate.Value, patientClinicalEventId))
                {
                    throw new DomainException("Duplication of adverse event. Please check onset and resolution dates");
                }
                else
                {
                    if (CheckEventOnsetDateWithinRange(sourceTerminology.Id, onsetDate.Value, patientClinicalEventId))
                    {
                        throw new DomainException("Duplication of adverse event. Please check onset and resolution dates");
                    }
                    else
                    {
                        if (resolutionDate.HasValue)
                        {
                            if (CheckEventOnsetDateWithNoResolutionDateBeforeOnset(sourceTerminology.Id, onsetDate.Value, patientClinicalEventId))
                            {
                                throw new DomainException("Duplication of adverse event. Please check onset and resolution dates");
                            }
                        }
                    }
                }

                // Check clinical event overlapping - RESOLUTION DATE
                if (resolutionDate.HasValue)
                {
                    if (CheckEventResolutionDateAgainstOnsetDateWithNoResolutionDate(sourceTerminology.Id, resolutionDate.Value, patientClinicalEventId))
                    {
                        throw new DomainException("Duplication of adverse event. Please check onset and resolution dates");
                    }
                    else
                    {
                        if (CheckEventResolutionDateWithinRange(sourceTerminology.Id, resolutionDate.Value, patientClinicalEventId))
                        {
                            throw new DomainException("Duplication of adverse event. Please check onset and resolution dates");
                        }
                    }
                }
            }

            patientClinicalEvent.ChangeClinicalEventDetails(onsetDate, resolutionDate, sourceTerminology, sourceDescription);
        }

        public void ChangeConditionDetails(int patientConditionId, int sourceTerminologyMedDraId, DateTime startDate, DateTime? outcomeDate, Outcome outcome, TreatmentOutcome treatmentOutcome, string comments)
        {
            var patientCondition = PatientConditions.SingleOrDefault(t => t.Id == patientConditionId);
            if (patientCondition == null)
            {
                throw new KeyNotFoundException(nameof(patientConditionId));
            }

            if (startDate < DateOfBirth)
            {
                throw new DomainException("Start Date should be after Date Of Birth");
            }

            if (outcomeDate.HasValue)
            {
                if (outcomeDate < DateOfBirth)
                {
                    throw new DomainException("Outcome Date should be after Date Of Birth");
                }
            }

            if (CheckConditionStartDateAgainstStartDateWithNoEndDate(sourceTerminologyMedDraId, startDate, patientConditionId))
            {
                throw new DomainException("Duplication of condition. Please check condition start and outcome dates");
            }
            else
            {
                if (CheckConditionStartDateWithinRange(sourceTerminologyMedDraId, startDate, patientConditionId))
                {
                    throw new DomainException("Duplication of condition. Please check condition start and outcome dates");
                }
                else
                {
                    if (outcomeDate.HasValue)
                    {
                        if (CheckConditionStartDateWithNoEndDateBeforeStart(sourceTerminologyMedDraId, startDate, patientConditionId))
                        {
                            throw new DomainException("Duplication of condition. Please check condition start and outcome dates");
                        }
                    }
                }
            }

            if (outcomeDate.HasValue)
            {
                if (CheckConditionEndDateAgainstStartDateWithNoEndDate(sourceTerminologyMedDraId, outcomeDate.Value, patientConditionId))
                {
                    throw new DomainException("Duplication of condition. Please check condition start and outcome dates");
                }
                else
                {
                    if (CheckConditionEndDateWithinRange(sourceTerminologyMedDraId, outcomeDate.Value, patientConditionId))
                    {
                        throw new DomainException("Duplication of condition. Please check condition start and outcome dates");
                    }
                }
            }

            patientCondition.ChangeConditionDetails(startDate, outcomeDate, outcome, treatmentOutcome, comments);
        }

        public void ChangeMedicationDetails(int patientMedicationId, DateTime startDate, DateTime? endDate, string dose, string doseFrequency, string doseUnit)
        {
            var patientMedication = PatientMedications.SingleOrDefault(t => t.Id == patientMedicationId);
            if (patientMedication == null)
            {
                throw new KeyNotFoundException(nameof(patientMedicationId));
            }

            if (startDate < DateOfBirth)
            {
                throw new DomainException("Start Date should be after Date Of Birth");
            }

            if (endDate.HasValue)
            {
                if (endDate < DateOfBirth)
                {
                    throw new DomainException("End Date should be after Date Of Birth");
                }
            }

            if (CheckMedicationStartDateAgainstStartDateWithNoEndDate(patientMedication.Concept.Id, startDate, patientMedicationId))
            {
                throw new DomainException("Duplication of medication. Please check start and end dates");
            }
            else
            {
                if (CheckMedicationStartDateWithinRange(patientMedication.Concept.Id, startDate, patientMedicationId))
                {
                    throw new DomainException("Duplication of medication. Please check start and end dates");
                }
                else
                {
                    if (endDate.HasValue)
                    {
                        if (CheckMedicationStartDateWithNoEndDateBeforeStart(patientMedication.Concept.Id, startDate, patientMedicationId))
                        {
                            throw new DomainException("Duplication of medication. Please check start and end dates");
                        }
                    }
                }
            }

            if (endDate.HasValue)
            {
                if (CheckMedicationEndDateAgainstStartDateWithNoEndDate(patientMedication.Concept.Id, endDate.Value, patientMedicationId))
                {
                    throw new DomainException("Duplication of medication. Please check start and end dates");
                }
                else
                {
                    if (CheckMedicationEndDateWithinRange(patientMedication.Concept.Id, endDate.Value, patientMedicationId))
                    {
                        throw new DomainException("Duplication of medication. Please check start and end dates");
                    }
                }
            }

            patientMedication.ChangeMedicationDetails(startDate, endDate, dose, doseFrequency, doseUnit);
        }

        public void ChangePatientStatus(PatientStatus patientStatus, DateTime effectiveDate, string comments)
        {
            var newPatientStatusHistory = new PatientStatusHistory(patientStatus, effectiveDate, comments);
            PatientStatusHistories.Add(newPatientStatusHistory);
        }

        public void ArchiveClinicalEvent(int patientClinicalEventId, string reason, User user)
        {
            var patientClinicalEvent = PatientClinicalEvents.SingleOrDefault(t => t.Id == patientClinicalEventId);
            if (patientClinicalEvent == null)
            {
                throw new KeyNotFoundException(nameof(patientClinicalEventId));
            }

            patientClinicalEvent.ArchiveClinicalEvent(user, reason);
        }

        public void ArchiveMedication(int patientMedicationId, string reason, User user)
        {
            var patientMedication = PatientMedications.SingleOrDefault(t => t.Id == patientMedicationId);
            if (patientMedication == null)
            {
                throw new KeyNotFoundException(nameof(patientMedicationId));
            }

            patientMedication.ArchiveMedication(user, reason);
        }

        private Boolean CheckConditionStartDateAgainstStartDateWithNoEndDate(int sourceTerminologyMedDraId, DateTime startDate, long patientConditionId)
        {
            if (patientConditionId > 0)
            {
                return PatientConditions
                        .OrderBy(pc => pc.OnsetDate)
                        .Where(pc => pc.Id != patientConditionId
                                && pc.TerminologyMedDra.Id == sourceTerminologyMedDraId
                                && pc.OnsetDate <= startDate
                                && pc.OutcomeDate == null
                                && pc.Archived == false)
                        .Any();
            }
            else
            {
                return PatientConditions
                        .OrderBy(pc => pc.OnsetDate)
                        .Where(pc => pc.TerminologyMedDra.Id == sourceTerminologyMedDraId
                                && pc.OnsetDate <= startDate
                                && pc.OutcomeDate == null
                                && pc.Archived == false)
                        .Any();
            }
        }

        private Boolean CheckConditionStartDateWithinRange(int sourceTerminologyMedDraId, DateTime startDate, long patientConditionId)
        {
            if (patientConditionId > 0)
            {
                return PatientConditions
                        .OrderBy(pc => pc.OnsetDate)
                        .Where(pc => pc.Id != patientConditionId
                                && pc.TerminologyMedDra.Id == sourceTerminologyMedDraId
                                && startDate >= pc.OnsetDate
                                && startDate <= pc.OutcomeDate
                                && pc.Archived == false)
                        .Any();
            }
            else
            {
                return PatientConditions
                        .OrderBy(pc => pc.OnsetDate)
                        .Where(pc => pc.TerminologyMedDra.Id == sourceTerminologyMedDraId
                                && startDate >= pc.OnsetDate
                                && startDate <= pc.OutcomeDate
                                && pc.Archived == false)
                        .Any();
            }
        }

        private Boolean CheckConditionStartDateWithNoEndDateBeforeStart(int sourceTerminologyMedDraId, DateTime startDate, long patientConditionId)
        {
            if (patientConditionId > 0)
            {
                return PatientConditions
                        .OrderBy(pc => pc.OnsetDate)
                        .Where(pc => pc.Id != patientConditionId
                                && pc.TerminologyMedDra.Id == sourceTerminologyMedDraId
                                && startDate < pc.OnsetDate
                                && pc.Archived == false)
                        .Any();
            }
            else
            {
                return PatientConditions
                        .OrderBy(pc => pc.OnsetDate)
                        .Where(pc => pc.TerminologyMedDra.Id == sourceTerminologyMedDraId
                                && startDate < pc.OnsetDate
                                && pc.Archived == false)
                        .Any();
            }
        }

        private Boolean CheckConditionEndDateAgainstStartDateWithNoEndDate(int sourceTerminologyMedDraId, DateTime outcomeDate, long patientConditionId)
        {
            if (patientConditionId > 0)
            {
                return PatientConditions
                        .OrderBy(pc => pc.OnsetDate)
                        .Where(pc => pc.Id != patientConditionId
                                && pc.TerminologyMedDra.Id == sourceTerminologyMedDraId
                                && pc.OnsetDate <= outcomeDate 
                                && pc.OutcomeDate == null
                                && pc.Archived == false)
                        .Any();
            }
            else
            {
                return PatientConditions
                        .OrderBy(pc => pc.OnsetDate)
                        .Where(pc => pc.TerminologyMedDra.Id == sourceTerminologyMedDraId
                                && pc.OnsetDate <= outcomeDate
                                && pc.OutcomeDate == null
                                && pc.Archived == false)
                        .Any();
            }
        }

        private Boolean CheckConditionEndDateWithinRange(int sourceTerminologyMedDraId, DateTime outcomeDate, long patientConditionId)
        {
            if (patientConditionId > 0)
            {
                return PatientConditions
                        .OrderBy(pc => pc.OnsetDate)
                        .Where(pc => pc.Id != patientConditionId
                                && pc.TerminologyMedDra.Id == sourceTerminologyMedDraId
                                && outcomeDate >= pc.OnsetDate
                                && outcomeDate <= pc.OutcomeDate 
                                && pc.Archived == false)
                        .Any();
            }
            else
            {
                return PatientConditions
                        .OrderBy(pc => pc.OnsetDate)
                        .Where(pc => pc.TerminologyMedDra.Id == sourceTerminologyMedDraId
                                && outcomeDate >= pc.OnsetDate
                                && outcomeDate <= pc.OutcomeDate
                                && pc.Archived == false)
                        .Any();
            }
        }

        private Boolean CheckEventOnsetDateAgainstOnsetDateWithNoResolutionDate(int sourceTerminologyMedDraId, DateTime onsetDate, long patientClinicalEventId)
        {
            if (patientClinicalEventId > 0)
            {
                return PatientClinicalEvents
                        .OrderBy(pc => pc.OnsetDate)
                        .Where(pc => pc.Id != patientClinicalEventId
                                && pc.SourceTerminologyMedDra.Id == sourceTerminologyMedDraId
                                && pc.OnsetDate <= onsetDate 
                                && pc.ResolutionDate == null
                                && pc.Archived == false)
                        .Any();
            }
            else
            {
                return PatientClinicalEvents
                        .OrderBy(pc => pc.OnsetDate)
                        .Where(pc => pc.SourceTerminologyMedDra.Id == sourceTerminologyMedDraId
                                && pc.OnsetDate <= onsetDate
                                && pc.ResolutionDate == null
                                && pc.Archived == false)
                        .Any();
            }
        }

        private Boolean CheckEventOnsetDateWithinRange(int sourceTerminologyMedDraId, DateTime onsetDate, long patientClinicalEventId)
        {
            if (patientClinicalEventId > 0)
            {
                return PatientClinicalEvents
                        .OrderBy(pc => pc.OnsetDate)
                        .Where(pc => pc.Id != patientClinicalEventId
                                && pc.SourceTerminologyMedDra.Id == sourceTerminologyMedDraId
                                && onsetDate >= pc.OnsetDate 
                                && onsetDate <= pc.ResolutionDate
                                && pc.Archived == false)
                        .Any();
            }
            else
            {
                return PatientClinicalEvents
                        .OrderBy(pc => pc.OnsetDate)
                        .Where(pc => pc.SourceTerminologyMedDra.Id == sourceTerminologyMedDraId
                                && onsetDate >= pc.OnsetDate
                                && onsetDate <= pc.ResolutionDate
                                && pc.Archived == false)
                        .Any();
            }
        }

        private Boolean CheckEventOnsetDateWithNoResolutionDateBeforeOnset(int sourceTerminologyMedDraId, DateTime onsetDate, long patientClinicalEventId)
        {
            if (patientClinicalEventId > 0)
            {
                return PatientClinicalEvents
                        .OrderBy(pc => pc.OnsetDate)
                        .Where(pc => pc.Id != patientClinicalEventId
                                && pc.SourceTerminologyMedDra.Id == sourceTerminologyMedDraId
                                && onsetDate < pc.OnsetDate
                                && pc.Archived == false)
                        .Any();
            }
            else
            {
                return PatientClinicalEvents
                        .OrderBy(pc => pc.OnsetDate)
                        .Where(pc => pc.SourceTerminologyMedDra.Id == sourceTerminologyMedDraId
                                && onsetDate < pc.OnsetDate
                                && pc.Archived == false)
                        .Any();
            }
        }

        private Boolean CheckEventResolutionDateAgainstOnsetDateWithNoResolutionDate(int sourceTerminologyMedDraId, DateTime resolutionDate, long patientClinicalEventId)
        {
            if (patientClinicalEventId > 0)
            {
                return PatientClinicalEvents
                        .OrderBy(pc => pc.OnsetDate)
                        .Where(pc => pc.Id != patientClinicalEventId
                                && pc.SourceTerminologyMedDra.Id == sourceTerminologyMedDraId
                                && pc.OnsetDate <= resolutionDate 
                                && pc.ResolutionDate == null
                                && pc.Archived == false)
                        .Any();
            }
            else
            {
                return PatientClinicalEvents
                        .OrderBy(pc => pc.OnsetDate)
                        .Where(pc => pc.SourceTerminologyMedDra.Id == sourceTerminologyMedDraId
                                && pc.OnsetDate <= resolutionDate
                                && pc.ResolutionDate == null
                                && pc.Archived == false)
                        .Any();
            }
        }

        private Boolean CheckEventResolutionDateWithinRange(int sourceTerminologyMedDraId, DateTime resolutionDate, long patientClinicalEventId)
        {
            if (patientClinicalEventId > 0)
            {
                return PatientClinicalEvents
                        .OrderBy(pc => pc.OnsetDate)
                        .Where(pc => pc.Id != patientClinicalEventId
                                && pc.SourceTerminologyMedDra.Id == sourceTerminologyMedDraId
                                && resolutionDate >= pc.OnsetDate 
                                && resolutionDate <= pc.ResolutionDate
                                && pc.Archived == false)
                        .Any();
            }
            else
            {
                return PatientClinicalEvents
                        .OrderBy(pc => pc.OnsetDate)
                        .Where(pc => pc.SourceTerminologyMedDra.Id == sourceTerminologyMedDraId
                                && resolutionDate >= pc.OnsetDate
                                && resolutionDate <= pc.ResolutionDate
                                && pc.Archived == false)
                        .Any();
            }
        }

        private Boolean CheckMedicationStartDateAgainstStartDateWithNoEndDate(int conceptId, DateTime startDate, long patientMedicationId)
        {
            if (patientMedicationId > 0)
            {
                return PatientMedications
                        .OrderBy(pc => pc.StartDate)
                        .Where(pc => pc.Id != patientMedicationId
                                && pc.Concept?.Id == conceptId
                                && pc.StartDate <= startDate 
                                && pc.EndDate == null 
                                && pc.Archived == false)
                        .Any();
            }
            else
            {
                return PatientMedications
                        .OrderBy(pc => pc.StartDate)
                        .Where(pc => pc.Concept?.Id == conceptId
                                && pc.StartDate <= startDate
                                && pc.EndDate == null
                                && pc.Archived == false)
                        .Any();
            }
        }

        private Boolean CheckMedicationStartDateWithinRange(int conceptId, DateTime startDate, long patientMedicationId)
        {
            if (patientMedicationId > 0)
            {
                return PatientMedications
                        .OrderBy(pc => pc.StartDate)
                        .Where(pc => pc.Id != patientMedicationId
                                && pc.Concept?.Id == conceptId
                                && startDate >= pc.StartDate
                                && startDate <= pc.EndDate 
                                && pc.Archived == false)
                        .Any();
            }
            else
            {
                return PatientMedications
                        .OrderBy(pc => pc.StartDate)
                        .Where(pc => pc.Concept?.Id == conceptId
                                && startDate >= pc.StartDate
                                && startDate <= pc.EndDate
                                && pc.Archived == false)
                        .Any();
            }
        }

        private Boolean CheckMedicationStartDateWithNoEndDateBeforeStart(int conceptId, DateTime startDate, long patientMedicationId)
        {
            if (patientMedicationId > 0)
            {
                return PatientMedications
                        .OrderBy(pc => pc.StartDate)
                        .Where(pc => pc.Id != patientMedicationId
                                && pc.Concept?.Id == conceptId
                                && startDate < pc.StartDate
                                && pc.Archived == false)
                        .Any();
            }
            else
            {
                return PatientMedications
                        .OrderBy(pc => pc.StartDate)
                        .Where(pc => pc.Concept?.Id == conceptId
                                && startDate < pc.StartDate
                                && pc.Archived == false)
                        .Any();
            }
        }

        private Boolean CheckMedicationEndDateAgainstStartDateWithNoEndDate(int conceptId, DateTime endDate, long patientMedicationId)
        {
            if (patientMedicationId > 0)
            {
                return PatientMedications
                        .OrderBy(pc => pc.StartDate)
                        .Where(pc => pc.Id != patientMedicationId
                                && pc.Concept?.Id == conceptId 
                                && pc.StartDate <= endDate 
                                && pc.EndDate == null 
                                && pc.Archived == false)
                        .Any();
            }
            else
            {
                return PatientMedications
                        .OrderBy(pc => pc.StartDate)
                        .Where(pc => pc.Concept?.Id == conceptId
                                && pc.StartDate <= endDate
                                && pc.EndDate == null
                                && pc.Archived == false)
                        .Any();
            }
        }

        private Boolean CheckMedicationEndDateWithinRange(int conceptId, DateTime endDate, long patientMedicationId)
        {
            if (patientMedicationId > 0)
            {
                return PatientMedications
                        .OrderBy(pc => pc.StartDate)
                        .Where(pc => pc.Id != patientMedicationId
                                && pc.Concept?.Id == conceptId 
                                && endDate >= pc.StartDate
                                && endDate <= pc.EndDate 
                                && pc.Archived == false)
                        .Any();
            }
            else
            {
                return PatientMedications
                        .OrderBy(pc => pc.StartDate)
                        .Where(pc => pc.Concept?.Id == conceptId
                                && endDate >= pc.StartDate
                                && endDate <= pc.EndDate
                                && pc.Archived == false)
                        .Any();
            }
        }
    }
}