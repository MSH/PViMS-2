using System.ComponentModel.DataAnnotations.Schema;

namespace PVIMS.Core.Entities
{
    [Table(nameof(ConditionLabTest))]
    public class ConditionLabTest : EntityBase
    {
        public virtual Condition Condition { get; set; }
        public virtual LabTest LabTest { get; set; }
    }
}