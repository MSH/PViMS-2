using System.Runtime.Serialization;

namespace PVIMS.API.Models
{
    /// <summary>
    /// A role representation DTO - IDENTIFIER ONLY
    /// </summary>
    [DataContract()]
    public class RoleIdentifierDto : LinkedResourceBaseDto
    {
        /// <summary>
        /// The unique Id of the role
        /// </summary>
        [DataMember]
        public int Id { get; set; }

        /// <summary>
        /// The name of the role
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// The key identifying the role
        /// </summary>
        [DataMember]
        public string Key { get; set; }
    }
}
