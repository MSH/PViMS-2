using System;
using System.ComponentModel.DataAnnotations;

namespace PVIMS.API.Models
{
    public class AppointmentForCreationDto
    {
        /// <summary>
        /// The date of the appointment
        /// </summary>
        public DateTime AppointmentDate { get; set; }

        /// <summary>
        /// The reason for the appointment
        /// </summary>
        [Required]
        [StringLength(250)]
        public string Reason { get; set; }
    }
}
