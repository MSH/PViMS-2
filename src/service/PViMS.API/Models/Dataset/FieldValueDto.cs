using System.Runtime.Serialization;

namespace PVIMS.API.Models
{
    /// <summary>
    /// Distinct list of field values associated to drop down and listbox element types
    /// </summary>
    [DataContract()]
    public class FieldValueDto
    {
        /// <summary>
        /// The unique id of the field value
        /// </summary>
        [DataMember]
        public int FieldValueId { get; set; }

        /// <summary>
        /// The value of the field
        /// </summary>
        [DataMember]
        public string Value { get; set; }
    }
}
