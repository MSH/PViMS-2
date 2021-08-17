using System.Collections.Generic;

namespace PVIMS.API.Models
{
    public class UserRolesForUpdateDto
    {
        /// <summary>
        /// Roles the user has been allocated to
        /// </summary>
        public List<string> Roles { get; set; }
    }
}
