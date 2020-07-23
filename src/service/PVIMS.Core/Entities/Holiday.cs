using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PVIMS.Core.Entities
{
    [Table(nameof(Holiday))]
    public class Holiday : EntityBase
    {
        [Required]
        public DateTime HolidayDate { get; set; }

        [Required]
        [StringLength(100)]
        public string Description { get; set; }
    }
}
