using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VPS.Common.Domain;

namespace PVIMS.Core.Entities
{
    [Table(nameof(Role))]
    public class Role : Entity<int>
    {
        [Required]
        [StringLength(30)]
        public string Name { get; set; }

        [Required]
        [StringLength(30, ErrorMessage = "{0} can be at most {1} characters long.")]
        public string Key { get; set; }
    }
}
