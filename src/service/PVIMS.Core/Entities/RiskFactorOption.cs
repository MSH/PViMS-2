using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PVIMS.Core.Entities
{
    [Table(nameof(RiskFactorOption))]
    public class RiskFactorOption : EntityBase
    {
        [Required]
        [StringLength(50)]
        public string OptionName { get; set; }
        [Required]
        [StringLength(250)]
        public string Criteria { get; set; }
        [StringLength(30)]
        public string Display { get; set; }
    }
}
