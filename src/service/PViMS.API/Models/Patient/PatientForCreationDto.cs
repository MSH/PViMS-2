using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PVIMS.API.Models
{
    public class PatientForCreationDto
    {
        /// <summary>
        /// The first name of the patient
        /// </summary>
        [Required]
        [StringLength(30)]
        public string FirstName { get; set; }

        /// <summary>
        /// The first name of the patient
        /// </summary>
        [Required]
        [StringLength(30)]
        public string LastName { get; set; }

        /// <summary>
        /// The first name of the patient
        /// </summary>
        [StringLength(30)]
        public string MiddleName { get; set; }

        /// <summary>
        /// The date of birth of the patient
        /// </summary>
        public DateTime DateOfBirth { get; set; }

        /// <summary>
        /// The facility that the patient is being registered against
        /// </summary>
        [Required]
        [StringLength(100)]
        public string FacilityName { get; set; }

        /// <summary>
        /// The primary condition group for the patient
        /// </summary>
        [Required]
        public int ConditionGroupId { get; set; }

        /// <summary>
        /// The meddra term that has been associated to the primary group that the patient belongs to
        /// </summary>
        [Required]
        public int MeddraTermId { get; set; }

        /// <summary>
        /// The cohort group the patient should be assigned to
        /// </summary>
        public int? CohortGroupId { get; set; }

        /// <summary>
        /// The date the patient should be enroled into this cohort
        /// </summary>
        public DateTime? EnroledDate { get; set; }

        /// <summary>
        /// The start date of the primary condition
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// The end date of the primary condition
        /// </summary>
        public DateTime? OutcomeDate { get; set; }

        /// <summary>
        /// Any comments associated to th
        /// </summary>
        [StringLength(100)]
        public string Comments { get; set; }

        /// <summary>
        /// Patient custom attributes
        /// </summary>
        public IDictionary<int, string> Attributes { get; set; }

        /// <summary>
        /// The type of encounter on patient registration
        /// </summary>
        [Required]
        public int EncounterTypeId { get; set; }

        /// <summary>
        /// The priority of the encounter
        /// </summary>
        [Required]
        public int PriorityId { get; set; }

        /// <summary>
        /// The date of the encounter
        /// </summary>
        public DateTime EncounterDate { get; set; }

    }
}
