using PVIMS.Core.CustomAttributes;
using PVIMS.Core.Entities.Accounts;
using PVIMS.Core.Utilities;
using System;

namespace PVIMS.Core.Entities
{
    public class PatientMedication : EntityBase, IExtendable
	{
        public PatientMedication()
        {
            PatientMedicationGuid = Guid.NewGuid();
        }

        public DateTime DateStart { get; set; }
        public DateTime? DateEnd { get; set; }
        public string Dose { get; set; }
        public string DoseFrequency { get; set; }
        public string DoseUnit { get; set; }
        public int PatientId { get; set; }
        public Guid PatientMedicationGuid { get; set; }
        public bool Archived { get; set; }
        public DateTime? ArchivedDate { get; set; }
        public string ArchivedReason { get; set; }
        public int? AuditUserId { get; set; }
        public string MedicationSource { get; set; }
        public int ConceptId { get; set; }
        public int? ProductId { get; set; }

        public virtual Concept Concept { get; set; }
        public virtual Product Product { get; set; }
		public virtual Patient Patient { get; set; }
        public virtual User AuditUser { get; set; }

        public string DisplayName
        {
            get
            {
                if(Product == null && Concept == null) { return ""; };
                return Product != null 
                    ? $"{Concept.ConceptName} ({Concept.MedicationForm.Description}) ({Product.ProductName}); {Dose} {DoseUnit}" 
                    : $"{Concept.ConceptName} ({Concept.MedicationForm.Description}); {Dose} {DoseUnit}";
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
    }
}