using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PVIMS.Core.Entities;

namespace PVIMS.Infrastructure.EntityConfigurations
{
    class FacilityTypeEntityTypeConfiguration : IEntityTypeConfiguration<FacilityType>
    {
        public void Configure(EntityTypeBuilder<FacilityType> configuration)
        {
            configuration.ToTable("FacilityType");

            configuration.HasKey(e => e.Id);

            configuration.Property(c => c.Description)
                .HasMaxLength(50)
                .IsRequired(true);

            configuration.HasIndex("Description").IsUnique(true);
        }
    }
}
