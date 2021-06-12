using System;
using PVIMS.Core.CustomAttributes;
using PVIMS.Core.Entities.Accounts;
using PVIMS.Core.Utilities;

namespace PVIMS.Core.Entities
{
    public class PatientLabTest : EntityBase, IExtendable
	{
        public PatientLabTest()
        {
            PatientLabTestGuid = Guid.NewGuid();
        }

        public DateTime TestDate { get; set; }
        public string TestResult { get; set; }
        public int LabTestId { get; set; }
        public int PatientId { get; set; }
        public Guid PatientLabTestGuid { get; set; }
        public int? TestUnitId { get; set; }
        public string LabValue { get; set; }
        public bool Archived { get; set; }
        public DateTime? ArchivedDate { get; set; }
        public string ArchivedReason { get; set; }
        public int? AuditUserId { get; set; }
        public string ReferenceLower { get; set; }
        public string ReferenceUpper { get; set; }
        public string LabTestSource { get; set; }

		public virtual LabTestUnit TestUnit { get; set; }
		public virtual LabTest LabTest { get; set; }
		public virtual Patient Patient { get; set; }
        public virtual User AuditUser { get; set; }

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