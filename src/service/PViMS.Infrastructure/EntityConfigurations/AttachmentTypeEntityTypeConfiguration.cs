using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PVIMS.Core.Entities;

namespace PVIMS.Infrastructure.EntityConfigurations
{
    class AttachmentTypeEntityTypeConfiguration : IEntityTypeConfiguration<AttachmentType>
    {
        public void Configure(EntityTypeBuilder<AttachmentType> configuration)
        {
            configuration.ToTable("AttachmentType");

            configuration.HasKey(e => e.Id);

            configuration.Property(c => c.Description)
                .HasMaxLength(50)
                .IsRequired(true);

            configuration.Property(c => c.Key)
                .HasMaxLength(4)
                .IsRequired(true);

            configuration.HasIndex("Description").IsUnique(true);
            configuration.HasIndex("Key").IsUnique(true);
        }
    }
}
