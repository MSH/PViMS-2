using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PVIMS.API.Models
{
    public class PatientClinicalEventForUpdateDto
    {
        /// <summary>
        /// The description of the source event
        /// </summary>
        [Required]
        [StringLength(250)]
        public string SourceDescription { get; set; }

        /// <summary>
        /// The unique identifier of the meddra term
        /// </summary>
        [Required]
        public int SourceTerminologyMedDraId { get; set; }

        /// <summary>
        /// The onset date of the clinical event
        /// </summary>
        public DateTime OnsetDate { get; set; }

        /// <summary>
        /// The resolution date of the clinical event
        /// </summary>
        public DateTime? ResolutionDate { get; set; }

        /// <summary>
        /// Clinical event custom attributes
        /// </summary>
        public IDictionary<int, string> Attributes { get; set; }
    }
}
