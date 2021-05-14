using PVIMS.Core.CustomAttributes;
using PVIMS.Core.Entities.Accounts;
using PVIMS.Core.Utilities;
using System;

namespace PVIMS.Core.Entities
{
    public class PatientMedication : EntityBase, IExtendable
	{
        protected PatientMedication()
        {
        }

        public PatientMedication(Concept concept, DateTime startDate, DateTime? endDate, string dose, string doseFrequency, string doseUnit, Product product, string medicationSource)
        {
            PatientMedicationGuid = Guid.NewGuid();
            Archived = false;

            Concept = concept;
            ConceptId = concept.Id;

            StartDate = startDate;
            EndDate = endDate;

            Dose = dose;
            DoseFrequency = doseFrequency;
            DoseUnit = doseUnit;

            if(product != null)
            {
                Product = product;
                ProductId = product.Id;
            }

            MedicationSource = medicationSource;
        }

        public void ChangeMedicationDetails(DateTime startDate, DateTime? endDate, string dose, string doseFrequency, string doseUnit)
        {
            StartDate = startDate;
            EndDate = endDate;

            Dose = dose;
            DoseFrequency = doseFrequency;
            DoseUnit = doseUnit;
        }

        public void ArchiveMedication(User user, string reason)
        {
            Archived = true;
            ArchivedDate = DateTime.Now;
            ArchivedReason = reason;
            AuditUser = user;
            AuditUserId = user.Id;
        }

        public DateTime StartDate { get; private set; }
        public DateTime? EndDate { get; private set; }
        public string Dose { get; private set; }
        public string DoseFrequency { get; private set; }
        public string DoseUnit { get; private set; }
        
        public int PatientId { get; private set; }
        public virtual Patient Patient { get; private set; }

        public Guid PatientMedicationGuid { get; private set; }
        public bool Archived { get; private set; }
        public DateTime? ArchivedDate { get; private set; }
        public string ArchivedReason { get; private set; }
        
        public int? AuditUserId { get; private set; }
        public virtual User AuditUser { get; private set; }
        
        public string MedicationSource { get; private set; }
        
        public int ConceptId { get; private set; }
        public virtual Concept Concept { get; private set; }

        public int? ProductId { get; private set; }
        public virtual Product Product { get; private set; }
        
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