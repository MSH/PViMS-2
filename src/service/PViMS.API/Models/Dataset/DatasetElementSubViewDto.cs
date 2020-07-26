using System.Runtime.Serialization;

namespace PVIMS.API.Models
{
    /// <summary>
    /// List of dataset elements associated to a dataset category
    /// </summary>
    [DataContract()]
    public class DatasetElementSubViewDto
    {
        /// <summary>
        /// The unique id of the dataset sub element
        /// </summary>
        [DataMember]
        public int DatasetElementSubId { get; set; }

        /// <summary>
        /// The name of the dataset sub element
        /// </summary>
        [DataMember]
        public string DatasetElementSubName { get; set; }

        /// <summary>
        /// The type of sub element
        /// </summary>
        [DataMember]
        public string DatasetElementSubType { get; set; }
    }
}
