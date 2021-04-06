using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PVIMS.Core.Entities.Keyless;

namespace PVIMS.Infrastructure.EntityConfigurations.KeyLess
{
    class AdverseEventQuarterlyListViewTypeConfiguration : IEntityTypeConfiguration<AdverseEventQuarterlyList>
    {
        public void Configure(EntityTypeBuilder<AdverseEventQuarterlyList> configuration)
        {
            configuration.ToView("vwAdverseEventQuarterlyList");

            configuration.HasNoKey();
        }
    }
}
