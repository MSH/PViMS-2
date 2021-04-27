using System.ComponentModel.DataAnnotations;

namespace PVIMS.API.Models
{
    public class ArchiveDto
    {
        /// <summary>
        /// The reason for archiving the resource
        /// </summary>
        [Required]
        [StringLength(200)]
        public string Reason { get; set; }
    }
}
