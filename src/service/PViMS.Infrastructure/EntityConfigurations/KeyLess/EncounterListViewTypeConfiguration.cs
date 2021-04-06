using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PVIMS.Core.Entities.Keyless;

namespace PVIMS.Infrastructure.EntityConfigurations.KeyLess
{
    class EncounterListViewTypeConfiguration : IEntityTypeConfiguration<EncounterList>
    {
        public void Configure(EntityTypeBuilder<EncounterList> configuration)
        {
            configuration.ToView("vwEncounterList");

            configuration.HasNoKey();
        }
    }
}
