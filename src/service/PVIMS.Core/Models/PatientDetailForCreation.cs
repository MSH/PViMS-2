using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using PVIMS.Core.CustomAttributes;

namespace PVIMS.Core.Models
{
    public class PatientDetailForCreation
    {
        public PatientDetailForCreation()
        {
            Conditions = new List<ConditionDetail>();
            LabTests = new List<LabTestDetail>();
            Medications = new List<MedicationDetail>();
            ClinicalEvents = new List<ClinicalEventDetail>();
            Attachments = new List<AttachmentDetail>();
        }

        [Required]
        [StringLength(30)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(30)]
        public string Surname { get; set; }

        [StringLength(30)]
        public string MiddleName { get; set; }

        [Required]
        [StringLength(100)]
        public string CurrentFacilityName { get; set; }

        [StringLength(1000)]
        public string Notes { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public int? CohortGroupId { get; set; }
        public DateTime? EnroledDate { get; set; }

        public int EncounterTypeId { get; set; }
        public int PriorityId { get; set; }
        public DateTime EncounterDate { get; set; }

        public List<CustomAttributeDetail> CustomAttributes { get; set; }

        public List<ConditionDetail> Conditions { get; set; }
        public List<LabTestDetail> LabTests { get; set; }
        public List<MedicationDetail> Medications { get; set; }
        public List<ClinicalEventDetail> ClinicalEvents { get; set; }
        public List<AttachmentDetail> Attachments { get; set; }

        public List<string> InvalidAttributes { get; set; }

        public bool IsValid()
        {
            var valid = true;

            // Ensure all required fields have a value
            foreach (var attributeDetail in CustomAttributes)
            {
                var value = attributeDetail.Value.ToString();
                if (attributeDetail.IsRequired)
                {
                    switch (attributeDetail.Type)
                    {
                        case CustomAttributeType.String:
                            if (String.IsNullOrWhiteSpace(value))
                            {
                                PublishMessage($"{attributeDetail.AttributeKey} is a required field and must be captured");
                                valid = false;
                            }
                            break;

                        case CustomAttributeType.Selection:
                            if (String.IsNullOrWhiteSpace(value) || value == "0")
                            {
                                PublishMessage($"{attributeDetail.AttributeKey} is a required field and must be captured");
                                valid = false;
                            }
                            break;

                        case CustomAttributeType.DateTime:
                            var dateTimeValue = Convert.ToDateTime(value);
                            if (String.IsNullOrWhiteSpace(value) || dateTimeValue == DateTime.MinValue)
                            {
                                PublishMessage($"{attributeDetail.AttributeKey} is a required field and must be captured");
                                valid = false;
                            }
                            break;

                        default:
                            break;
                    }
                }
            }

            return valid;
        }

        public void SetAttributeValue(string attributeKey, string attributeValue)
        {
            var attributeDetail = this.CustomAttributes.SingleOrDefault(ca => ca.AttributeKey == attributeKey);
            if(attributeDetail == null)
            {
                throw new ArgumentNullException(attributeKey);
            }

            attributeDetail.Value = attributeValue;
        }

        private void PublishMessage(string message)
        {
            if (InvalidAttributes == null)
            {
                InvalidAttributes = new List<string>();
            }
            if(!InvalidAttributes.Contains(message))
            {
                InvalidAttributes.Add(message);
            }
        }
    }

}
