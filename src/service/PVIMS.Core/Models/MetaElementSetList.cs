using System;

using VPS.Common.Domain;

namespace PVIMS.Core.Models
{
    public class MetaElementSetList : Entity<int>
    {
        public string Element { get; set; }
        public Int64 Value { get; set; }
    }
}
