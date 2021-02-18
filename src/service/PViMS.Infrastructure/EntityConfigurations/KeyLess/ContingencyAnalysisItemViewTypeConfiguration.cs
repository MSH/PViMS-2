using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PVIMS.Core.Entities.Keyless;

namespace PVIMS.Infrastructure.EntityConfigurations.KeyLess
{
    class ContingencyAnalysisItemViewTypeConfiguration : IEntityTypeConfiguration<ContingencyAnalysisItem>
    {
        public void Configure(EntityTypeBuilder<ContingencyAnalysisItem> configuration)
        {
            configuration.ToView("vwContingencyAnalysisItem");

            configuration.HasNoKey();
        }
    }
}
