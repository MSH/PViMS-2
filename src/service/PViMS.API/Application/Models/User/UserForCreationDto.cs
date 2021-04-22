using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PVIMS.API.Models
{
    public class UserForCreationDto
    {
        /// <summary>
        /// The email address of the user
        /// </summary>
        [EmailAddress]
        [StringLength(150)]
        public string Email { get; set; }

        [Required]
        [StringLength(100)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }

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
        /// Roles the user has been allocated to
        /// </summary>
        public List<string> Roles { get; set; }

        /// <summary>
        /// Facilities the user has been allocated to
        /// </summary>
        public List<string> Facilities { get; set; }
    }
}
