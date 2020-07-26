using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PVIMS.API.Models
{
    public class PatientLabTestForUpdateDto
    {
        /// <summary>
        /// The look up value for the lab test
        /// </summary>
        [Required]
        [StringLength(50)]
        public string LabTest { get; set; }

        /// <summary>
        /// The date the test was conducted
        /// </summary>
        public DateTime TestDate { get; set; }

        /// <summary>
        /// Test result - coded
        /// </summary>
        [StringLength(50)]
        public string TestResultCoded { get; set; }

        /// <summary>
        /// Test result - value
        /// </summary>
        [StringLength(20)]
        public string TestResultValue { get; set; }

        /// <summary>
        /// The look up value for the test unit
        /// </summary>
        [StringLength(50)]
        public string TestUnit { get; set; }

        /// <summary>
        /// The lower range of the test result
        /// </summary>
        [StringLength(20)]
        public string ReferenceLower { get; set; }

        /// <summary>
        /// The upper range of the test result
        /// </summary>
        [StringLength(20)]
        public string ReferenceUpper { get; set; }

        /// <summary>
        /// Lab test custom attributes
        /// </summary>
        public IDictionary<int, string> Attributes { get; set; }
    }
}
