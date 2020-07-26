using PVIMS.Core.ValueTypes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PVIMS.Core.Entities
{
    [Table(nameof(DatasetRule))]
    public partial class DatasetRule : EntityBase
    {
        public virtual Dataset Dataset { get; set; }
        public virtual DatasetElement DatasetElement { get; set; }

        [Required]
        public virtual DatasetRuleType RuleType { get; set; }

        public bool RuleActive { get; set; }
    }
}
