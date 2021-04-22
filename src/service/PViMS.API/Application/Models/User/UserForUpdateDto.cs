using PVIMS.API.Infrastructure.Attributes;
using PVIMS.API.Models.ValueTypes;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PVIMS.API.Models
{
    public class UserForUpdateDto
    {
        /// <summary>
        /// The email address of the user
        /// </summary>
        [EmailAddress]
        [StringLength(150)]
        public string Email { get; set; }

        /// <summary>
        /// The username of the user
        /// </summary>
        [Required]
        [StringLength(30)]
        public string UserName { get; set; }

        /// <summary>
        /// The first name of the user
        /// </summary>
        [Required]
        [StringLength(30)]
        public string FirstName { get; set; }

        /// <summary>
        /// The last name of the user
        /// </summary>
        [Required]
        [StringLength(30)]
        public string LastName { get; set; }

        /// <summary>
        /// Is the user able to download the analytical dataset
        /// </summary>
        [ValidEnumValue]
        public YesNoValueType AllowDatasetDownload { get; set; }

        /// <summary>
        /// Is the user account currently active
        /// </summary>
        [ValidEnumValue]
        public YesNoValueType Active { get; set; }

        /// <summary>
        /// Roles the user has been allocated to
        /// </summary>
        public List<string> Roles { get; set; }

        /// <summary>
        /// Facilities the user has been allocated to
        /// </summary>
        public List<string> Facilities { get; set; }
    }
}
