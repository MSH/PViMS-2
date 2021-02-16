using System.Runtime.Serialization;

namespace PVIMS.API.Models
{
    /// <summary>
    /// Distinct value list associated to a dataset element
    /// </summary>
    [DataContract()]
    public class GroupValueDto
    {
        /// <summary>
        /// The value of the dataset element being grouped
        /// </summary>
        [DataMember]
        public string GroupValue { get; set; }

        /// <summary>
        /// The number of records grouped
        /// </summary>
        [DataMember]
        public int Count { get; set; }
    }
}
