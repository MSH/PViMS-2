using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PVIMS.Core.Entities
{
    [Table(nameof(ContextType))]
    public class ContextType : EntityBase
    {
        public ContextType()
        {
            Datasets = new HashSet<Dataset>();
        }

        [Required]
        [StringLength(50)]
        public string Description { get; set; }

        public virtual ICollection<Dataset> Datasets { get; set; }
    }
}
