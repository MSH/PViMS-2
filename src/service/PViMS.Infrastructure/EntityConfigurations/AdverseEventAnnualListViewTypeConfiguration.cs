using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PVIMS.Core.Entities;
using PVIMS.Core.Entities.Keyless;

namespace PVIMS.Infrastructure.EntityConfigurations
{
    class AdverseEventAnnualListViewTypeConfiguration : IEntityTypeConfiguration<AdverseEventAnnualList>
    {
        public void Configure(EntityTypeBuilder<AdverseEventAnnualList> configuration)
        {
            configuration.ToView("vwAdverseEventAnnualList");

            configuration.HasNoKey();
        }
    }
}
