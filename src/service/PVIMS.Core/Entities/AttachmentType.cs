using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PVIMS.Core.Entities
{
    [Table(nameof(AttachmentType))]
    public partial class AttachmentType : EntityBase
    {
        public AttachmentType()
        {
        }

        [Required]
        [StringLength(50)]
        public string Description { get; set; }

        [Required]
        [StringLength(4)]
        public string Key { get; set; }
    }
}
