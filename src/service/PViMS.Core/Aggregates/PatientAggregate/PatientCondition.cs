using System;
using OpenXmlPowerTools;
using PVIMS.Core.Aggregates.UserAggregate;
using PVIMS.Core.CustomAttributes;
using PVIMS.Core.Utilities;

namespace PVIMS.Core.Entities
{
    public class PatientCondition : EntityBase, IExtendable
    {
        protected PatientCondition()
        {
        }

        public PatientCondition(DateTime onsetDate, DateTime? outcomeDate, TerminologyMedDra sourceTerminologyMedDra, Outcome outcome, TreatmentOutcome treatmentOutcome, string caseNumber, string comments, string conditionSource)
        {
            PatientConditionGuid = Guid.NewGuid();
            Archived = false;

            OnsetDate = onsetDate;
            OutcomeDate = outcomeDate;

            if (sourceTerminologyMedDra != null)
            {
                TerminologyMedDraId = sourceTerminologyMedDra.Id;
                TerminologyMedDra = sourceTerminologyMedDra;
            }

            if (outcome != null)
            {
                OutcomeId = outcome.Id;
                Outcome = outcome;
            }

            if (treatmentOutcome != null)
            {
                TreatmentOutcomeId = treatmentOutcome.Id;
                TreatmentOutcome = treatmentOutcome;
            }


            CaseNumber = caseNumber;
            Comments = comments;

            ConditionSource = conditionSource;
        }

        public void ChangeConditionDetails(DateTime onsetDate, DateTime? outcomeDate, Outcome outcome, TreatmentOutcome treatmentOutcome, string caseNumber, string comments)
        {
            OnsetDate = onsetDate;
            OutcomeDate = outcomeDate;

            if (outcome != null)
            {
                OutcomeId = outcome.Id;
                Outcome = outcome;
            }

            if (treatmentOutcome != null)
            {
                TreatmentOutcomeId = treatmentOutcome.Id;
                TreatmentOutcome = treatmentOutcome;
            }

            CaseNumber = caseNumber;
            Comments = comments;
        }

        public void Archive(User user, string reason)
        {
            Archived = true;
            ArchivedDate = DateTime.Now;
            ArchivedReason = reason;
            AuditUser = user;
            AuditUserId = user.Id;
        }

        public DateTime OnsetDate { get; private set; }
        public DateTime? OutcomeDate { get; private set; }

        public int? ConditionId { get; private set; }
        public virtual Condition Condition { get; private set; }

        public int? OutcomeId { get; private set; }
        public virtual Outcome Outcome { get; private set; }

        public int? TreatmentOutcomeId { get; private set; }
        public virtual TreatmentOutcome TreatmentOutcome { get; private set; }

        public string ConditionSource { get; private set; }
        public string CaseNumber { get; private set; }

        public int PatientId { get; private set; }
        public virtual Patient Patient { get; private set; }

        public Guid PatientConditionGuid { get; private set; }
        public string Comments { get; private set; }

        public int? TerminologyMedDraId { get; private set; }
        public virtual TerminologyMedDra TerminologyMedDra { get; private set; }

        public bool Archived { get; private set; }
        public DateTime? ArchivedDate { get; private set; }
        public string ArchivedReason { get; private set; }

        public int? AuditUserId { get; private set; }
        public virtual User AuditUser { get; private set; }

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

        public void ValidateAndSetAttributeValue<T>(CustomAttributeDetail attributeDetail, T attributeValue, string updatedByUser)
        {
            customAttributes.ValidateAndSetAttributeValue(attributeDetail, attributeValue, updatedByUser);
        }

        public DateTime GetUpdatedDate(string attributeKey)
        {
            return customAttributes.GetUpdatedDate(attributeKey);
        }

        public string GetUpdatedByUser(string attributeKey)
        {
            return customAttributes.GetUpdatedByUser(attributeKey);
        }
    }
}