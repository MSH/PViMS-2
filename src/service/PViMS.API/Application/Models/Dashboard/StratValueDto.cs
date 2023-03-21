using System.Runtime.Serialization;

namespace PVIMS.API.Models
{
    /// <summary>
    /// Stratiion value set for dashboard execution
    /// </summary>
    [DataContract()]
    public class StratValueDto
    {
        /// <summary>
        /// Axis or non axis based series data
        /// </summary>
        [DataMember]
        public string Strat { get; set; }

        /// <summary>
        /// Chart configurations
        /// </summary>
        [DataMember]
        public int StratValue { get; set; }
    }
}
