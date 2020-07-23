using VPS.Common.Domain;

namespace PVIMS.Core.Entities
{
    public class SelectionDataItem : Entity<int>
    {
        public string AttributeKey { get; set; }
        public string SelectionKey { get; set; }
        public string Value { get; set; }
    }
}
