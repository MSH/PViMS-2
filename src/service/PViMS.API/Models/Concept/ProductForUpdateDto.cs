using PVIMS.API.Attributes;
using PVIMS.API.Models.ValueTypes;
using System.ComponentModel.DataAnnotations;

namespace PVIMS.API.Models
{
    public class ProductForUpdateDto
    {
        /// <summary>
        /// The name of the concept
        /// </summary>
        [Required]
        [StringLength(250)]
        public string ConceptName { get; set; }

        /// <summary>
        /// The form of the concept
        /// </summary>
        [Required]
        [StringLength(50)]
        public string MedicationForm { get; set; }

        /// <summary>
        /// The name of the product
        /// </summary>
        [Required]
        [StringLength(200)]
        public string ProductName { get; set; }

        /// <summary>
        /// The name of the manufacturer
        /// </summary>
        [Required]
        [StringLength(200)]
        public string Manufacturer { get; set; }

        /// <summary>
        /// A general description of the product
        /// </summary>
        [StringLength(1000)]
        public string Description { get; set; }

        /// <summary>
        /// Is this product currently active
        /// </summary>
        [Required]
        [ValidEnumValue]
        public YesNoValueType Active { get; set; }
    }
}
