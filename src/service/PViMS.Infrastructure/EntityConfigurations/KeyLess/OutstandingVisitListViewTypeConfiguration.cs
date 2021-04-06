using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PVIMS.Core.Entities.Keyless;

namespace PVIMS.Infrastructure.EntityConfigurations.KeyLess
{
    class OutstandingVisitListViewTypeConfiguration : IEntityTypeConfiguration<OutstandingVisitList>
    {
        public void Configure(EntityTypeBuilder<OutstandingVisitList> configuration)
        {
            configuration.ToView("vwOutstandingVisitList");

            configuration.HasNoKey();
        }
    }
}
