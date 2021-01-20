using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PVIMS.Core.Entities;

namespace PVIMS.Infrastructure.EntityConfigurations
{
    class EncounterTypeEntityTypeConfiguration : IEntityTypeConfiguration<EncounterType>
    {
        public void Configure(EntityTypeBuilder<EncounterType> configuration)
        {
            configuration.ToTable("EncounterType");

            configuration.HasKey(e => e.Id);

            configuration.Property(c => c.Description)
                .HasMaxLength(50)
                .IsRequired(true);

            configuration.Property(c => c.Help)
                .HasMaxLength(250)
                .IsRequired(false);

            configuration.Property(c => c.Chronic)
                .HasDefaultValue(false)
                .IsRequired(true);

            configuration.HasIndex("Description").IsUnique(true);
        }
    }
}
