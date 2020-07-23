using VPS.Common.Domain;

namespace PVIMS.Core.Models
{
    public class ContingencyAnalysisList : Entity<int>
    {
        public int TerminologyMeddra_Id { get; set; }
        public string MeddraTerm { get; set; }
    }
}
