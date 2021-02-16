using System.Collections.Generic;
using System.Runtime.Serialization;

namespace PVIMS.API.Models
{
    /// <summary>
    /// A dataset element representation DTO - EXPANDED DETAILS
    /// </summary>
    [DataContract()]
    public class DatasetElementExpandedDto : DatasetElementDetailDto
    {
        /// <summary>
        /// Does this element implement the single dataset rule
        /// </summary>
        [DataMember]
        public string SingleDatasetRule { get; set; }

        /// <summary>
        /// A list of field values associated to the element (if applicable)
        /// </summary>
        [DataMember]
        public ICollection<FieldValueDto> FieldValues { get; set; } = new List<FieldValueDto>();
    }
}
