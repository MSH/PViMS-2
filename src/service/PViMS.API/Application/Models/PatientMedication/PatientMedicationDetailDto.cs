using System.Collections.Generic;
using System.Runtime.Serialization;

namespace PVIMS.API.Models
{
    /// <summary>
    /// A patient medication representation DTO - FULL DETAILS
    /// </summary>
    [DataContract()]
    public class PatientMedicationDetailDto : PatientMedicationIdentifierDto
    {
        /// <summary>
        /// The source description of the medication
        /// </summary>
        [DataMember]
        public string SourceDescription { get; set; }

        /// <summary>
        /// The unqiue identifier of the medication concept
        /// </summary>
        [DataMember]
        public long ConceptId { get; set; }

        /// <summary>
        /// The unqiue identifier of the medication product
        /// </summary>
        [DataMember]
        public long? ProductId { get; set; }

        /// <summary>
        /// The display name for the medication (based on concept and product)
        /// </summary>
        [DataMember]
        public string Medication { get; set; }

        /// <summary>
        /// The dose of the medication
        /// </summary>
        [DataMember]
        public string Dose { get; set; }

        /// <summary>
        /// The dose unit of the medication
        /// </summary>
        [DataMember]
        public string DoseUnit { get; set; }

        /// <summary>
        /// The dosing frequency of the medication
        /// </summary>
        [DataMember]
        public string DoseFrequency { get; set; }

        /// <summary>
        /// The start date of the medication
        /// </summary>
        [DataMember]
        public string StartDate { get; set; }

        /// <summary>
        /// The end date of the medication
        /// </summary>
        [DataMember]
        public string EndDate { get; set; }

        /// <summary>
        /// The indication type of the medication
        /// </summary>
        [DataMember]
        public string IndicationType { get; set; }

        /// <summary>
        /// A list of custom attributes associated to the patient medication
        /// </summary>
        [DataMember]
        public ICollection<AttributeValueDto> MedicationAttributes { get; set; } = new List<AttributeValueDto>();
    }
}
