using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PVIMS.Core.Entities;

namespace PVIMS.Infrastructure.EntityConfigurations
{
    class FieldEntityTypeConfiguration : IEntityTypeConfiguration<Field>
    {
        public void Configure(EntityTypeBuilder<Field> configuration)
        {
            configuration.ToTable("Field");

            configuration.HasKey(e => e.Id);

            configuration.Property(c => c.Mandatory)
                .HasDefaultValue(false)
                .IsRequired(true);

            configuration.Property(c => c.MaxLength)
                .IsRequired(false);

            configuration.Property(c => c.RegEx)
                .HasMaxLength(100)
                .IsRequired(false);

            configuration.Property(c => c.Decimals)
                .IsRequired(false);

            configuration.Property(c => c.MaxSize)
                .HasPrecision(18,2)
                .IsRequired(false);

            configuration.Property(c => c.MinSize)
                .HasPrecision(18, 2)
                .IsRequired(false);

            configuration.Property(c => c.Calculation)
                .HasMaxLength(100)
                .IsRequired(false);

            configuration.Property(c => c.Image)
                .HasColumnType("Image")
                .IsRequired(false);

            configuration.Property(c => c.FileSize)
                .IsRequired(false);

            configuration.Property(c => c.FileExt)
                .HasMaxLength(100)
                .IsRequired(false);

            configuration.Property(c => c.Anonymise)
                .HasDefaultValue(false)
                .IsRequired(true);

            configuration.HasOne(c => c.FieldType)
                .WithMany()
                .HasForeignKey("FieldType_Id")
                .IsRequired(true);

            configuration.HasMany(c => c.FieldValues)
               .WithOne()
               .HasForeignKey("Field_Id")
               .IsRequired(true)
               .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
