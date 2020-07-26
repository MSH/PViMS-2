using System.ComponentModel.DataAnnotations;

namespace PVIMS.API.Models
{
    public class ContactForUpdateDto
    {
        /// <summary>
        /// The first name of the contact at the organisation
        /// </summary>
        [Required]
        [StringLength(30)]
        public string ContactFirstName { get; set; }

        /// <summary>
        /// The last name of the contact at the organisation
        /// </summary>
        [Required]
        [StringLength(30)]
        public string ContactLastName { get; set; }

        /// <summary>
        /// The name of the organisation
        /// </summary>
        [Required]
        [StringLength(50)]
        public string OrganisationName { get; set; }

        /// <summary>
        /// The street address of the organisation
        /// </summary>
        [Required]
        [StringLength(100)]
        public string StreetAddress { get; set; }

        /// <summary>
        /// The city the organisation is located in
        /// </summary>
        [StringLength(50)]
        public string City { get; set; }

        /// <summary>
        /// The state the organisation is located in
        /// </summary>
        [StringLength(50)]
        public string State { get; set; }

        /// <summary>
        /// The post code the organisation is located in
        /// </summary>
        [StringLength(20)]
        public string PostCode { get; set; }

        /// <summary>
        /// The contact number of the organisation
        /// </summary>
        [StringLength(50)]
        public string ContactNumber { get; set; }

        /// <summary>
        /// The email address of the organisation
        /// </summary>
        [StringLength(50)]
        public string ContactEmail { get; set; }

        /// <summary>
        /// The country code the organisation is located in
        /// </summary>
        [StringLength(10)]
        public string CountryCode { get; set; }
    }
}
