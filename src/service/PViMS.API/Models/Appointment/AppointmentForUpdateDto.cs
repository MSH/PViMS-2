using PVIMS.API.Infrastructure.Attributes;
using PVIMS.API.Models.ValueTypes;
using System;
using System.ComponentModel.DataAnnotations;

namespace PVIMS.API.Models
{
    public class AppointmentForUpdateDto
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

        /// <summary>
        /// Has the appointment been cancelled
        /// </summary>
        [ValidEnumValue]
        public YesNoValueType Cancelled { get; set; }

        /// <summary>
        /// The reason for the cancellation if it has been cancelled
        /// </summary>
        [StringLength(250)]
        public string CancellationReason { get; set; }
    }
}
