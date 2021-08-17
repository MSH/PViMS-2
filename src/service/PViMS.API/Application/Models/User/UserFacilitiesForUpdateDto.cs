using System.Collections.Generic;

namespace PVIMS.API.Models
{
    public class UserFacilitiesForUpdateDto
    {
        /// <summary>
        /// Facilities the user has been allocated to
        /// </summary>
        public List<string> Facilities { get; set; }
    }
}
