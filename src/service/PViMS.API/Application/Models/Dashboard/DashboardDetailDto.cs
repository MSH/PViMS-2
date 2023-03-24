using System.Runtime.Serialization;

namespace PVIMS.API.Models
{
    /// <summary>
    /// A dashboard representation DTO - FULL DETAILS
    /// </summary>
    [DataContract()]
    public class DashboardDetailDto : LinkedResourceBaseDto
    {
        /// <summary>
        /// The unique Id of the dashboard
        /// </summary>
        [DataMember]
        public int Id { get; set; }

        /// <summary>
        /// A unique identifier for the dashboard
        /// </summary>
        [DataMember]
        public string UID { get; set; }

        /// <summary>
        /// The name of the dashboard
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// The short name of the dashboard
        /// </summary>
        [DataMember]
        public string ShortName { get; set; }

        /// <summary>
        /// The long name of the dashboard
        /// </summary>
        [DataMember]
        public string LongName { get; set; }

        /// <summary>
        /// Frequency of the dashboard
        /// </summary>
        [DataMember]
        public string Frequency { get; set; }

        /// <summary>
        /// Is this dashboard currently active
        /// </summary>
        [DataMember]
        public string Active { get; set; }

        /// <summary>
        /// Details of the encounter creation
        /// </summary>
        [DataMember]
        public string CreatedDetail { get; set; }

        /// <summary>
        /// Details of the last update to the encounter
        /// </summary>
        [DataMember]
        public string UpdatedDetail { get; set; }

        /// <summary>
        /// Icon associated to the dashboard
        /// </summary>
        [DataMember]
        public string Icon { get; set; }
    }
}