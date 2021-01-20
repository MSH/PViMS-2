using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PVIMS.Core.Entities;

namespace PVIMS.Infrastructure.EntityConfigurations
{
    class CareEventEntityTypeConfiguration : IEntityTypeConfiguration<CareEvent>
    {
        public void Configure(EntityTypeBuilder<CareEvent> configuration)
        {
            configuration.ToTable("CareEvent");

            configuration.HasKey(e => e.Id);

            configuration.Property(c => c.Description)
                .HasMaxLength(50)
                .IsRequired(true);

            configuration.HasIndex("Description").IsUnique(true);
        }
    }
}
