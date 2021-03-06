﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PVIMS.API.Models
{
    public class PatientMedicationForUpdateDto
    {
        /// <summary>
        /// The description of the source medication
        /// </summary>
        [Required]
        [StringLength(200)]
        public string SourceDescription { get; set; }

        /// <summary>
        /// The unique identifier of the medication concept
        /// </summary>
        [Required]
        public int ConceptId { get; set; }

        /// <summary>
        /// The unique identifier of the medication product
        /// </summary>
        public int? ProductId { get; set; }

        /// <summary>
        /// The start date of the medication
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// The end date of the medication
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// The dose of the medication
        /// </summary>
        [StringLength(30)]
        public string Dose { get; set; }

        /// <summary>
        /// The dose frequency of the medication
        /// </summary>
        [StringLength(30)]
        public string DoseFrequency { get; set; }

        /// <summary>
        /// The unit of the dose
        /// </summary>
        [StringLength(10)]
        public string DoseUnit { get; set; }

        /// <summary>
        /// Condition custom attributes
        /// </summary>
        public IDictionary<int, string> Attributes { get; set; }
    }
}
