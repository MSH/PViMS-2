using PVIMS.API.Attributes;
using PVIMS.API.Models.ValueTypes;
using System.ComponentModel.DataAnnotations;

namespace PVIMS.API.Models
{
    public class ConceptForUpdateDto
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
        /// Is this concept currently active
        /// </summary>
        [Required]
        [ValidEnumValue]
        public YesNoValueType Active { get; set; }
    }
}
