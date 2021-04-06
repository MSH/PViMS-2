using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PVIMS.Core.Entities.Keyless;

namespace PVIMS.Infrastructure.EntityConfigurations.KeyLess
{
    class CausalityNotSetListViewTypeConfiguration : IEntityTypeConfiguration<CausalityNotSetList>
    {
        public void Configure(EntityTypeBuilder<CausalityNotSetList> configuration)
        {
            configuration.ToView("vwCausalityNotSetList");

            configuration.HasNoKey();
        }
    }
}
