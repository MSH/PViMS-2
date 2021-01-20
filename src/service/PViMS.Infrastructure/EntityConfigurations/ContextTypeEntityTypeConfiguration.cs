using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PVIMS.Core.Entities;

namespace PVIMS.Infrastructure.EntityConfigurations
{
    class ContextTypeEntityTypeConfiguration : IEntityTypeConfiguration<ContextType>
    {
        public void Configure(EntityTypeBuilder<ContextType> configuration)
        {
            configuration.ToTable("ContextType");

            configuration.HasKey(e => e.Id);

            configuration.Property(c => c.Description)
                .HasMaxLength(50)
                .IsRequired(true);

            configuration.HasIndex("Description").IsUnique(true);
        }
    }
}
