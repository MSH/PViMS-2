﻿using System.ComponentModel.DataAnnotations;

namespace PVIMS.API.Models
{
    public class LabTestUnitForUpdateDto
    {
        /// <summary>
        /// The name of the lab test unit
        /// </summary>
        [Required]
        [StringLength(50)]
        public string LabTestUnitName { get; set; }
    }
}
