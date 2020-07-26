using System.Runtime.Serialization;

namespace PVIMS.API.Models
{
    /// <summary>
    /// A user role representation DTO
    /// </summary>
    [DataContract()]
    public class UserRoleDto
    {
        /// <summary>
        /// The unique Id of the user role
        /// </summary>
        [DataMember]
        public int Id { get; set; }

        /// <summary>
        /// The role key
        /// </summary>
        [DataMember]
        public string Key { get; set; }
    }
}
