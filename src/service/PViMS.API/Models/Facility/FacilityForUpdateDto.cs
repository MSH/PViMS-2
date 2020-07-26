using System.ComponentModel.DataAnnotations;

namespace PVIMS.API.Models
{
    public class FacilityForUpdateDto
    {
        /// <summary>
        /// The name of the facility
        /// </summary>
        [Required]
        [StringLength(100)]
        public string FacilityName { get; set; }

        /// <summary>
        /// The code of the facility
        /// </summary>
        [Required]
        [StringLength(10)]
        public string FacilityCode { get; set; }

        /// <summary>
        /// The type of the facility
        /// </summary>
        [Required]
        public string FacilityType { get; set; }

        /// <summary>
        /// The telephone number
        /// </summary>
        [StringLength(30)]
        public string ContactNumber { get; set; }

        /// <summary>
        /// The mobile number
        /// </summary>
        [StringLength(30)]
        public string MobileNumber { get; set; }

        /// <summary>
        /// The fax number for the facility
        /// </summary>
        [StringLength(30)]
        public string FaxNumber { get; set; }
    }
}
