using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PVIMS.Core.Entities;

namespace PVIMS.Infrastructure.EntityConfigurations
{
    class FieldTypeEntityTypeConfiguration : IEntityTypeConfiguration<FieldType>
    {
        public void Configure(EntityTypeBuilder<FieldType> configuration)
        {
            configuration.ToTable("FieldType");

            configuration.HasKey(e => e.Id);

            configuration.Property(c => c.Description)
                .HasMaxLength(50)
                .IsRequired(true);

            configuration.HasIndex("Description").IsUnique(true);
        }
    }
}
