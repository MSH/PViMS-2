using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PVIMS.Core.CustomAttributes;
using PVIMS.Core.Entities;

namespace PVIMS.Infrastructure.EntityConfigurations
{
    class CustomAttributeConfigurationEntityTypeConfiguration : IEntityTypeConfiguration<CustomAttributeConfiguration>
    {
        public void Configure(EntityTypeBuilder<CustomAttributeConfiguration> configuration)
        {
            configuration.ToTable("CustomAttributeConfiguration");

            configuration.HasKey(e => e.Id);

            configuration.Property(c => c.ExtendableTypeName)
                .IsRequired(true);

            configuration.Property(c => c.CustomAttributeType)
                .HasConversion(x => (int)x, x => (CustomAttributeType)x);

            configuration.Property(c => c.Category)
                .IsRequired(false);

            configuration.Property(c => c.AttributeKey)
                .IsRequired(true);

            configuration.Property(c => c.IsRequired)
                .HasDefaultValue(false)
                .IsRequired(true);

            configuration.Property(c => c.StringMaxLength)
                .IsRequired(false);

            configuration.Property(c => c.NumericMinValue)
                .IsRequired(false);

            configuration.Property(c => c.NumericMaxValue)
                .IsRequired(false);

            configuration.Property(c => c.FutureDateOnly)
                .HasDefaultValue(false)
                .IsRequired(true);

            configuration.Property(c => c.PastDateOnly)
                .HasDefaultValue(false)
                .IsRequired(true);

            configuration.Property(c => c.IsSearchable)
                .HasDefaultValue(false)
                .IsRequired(true);

            configuration.Property(c => c.AttributeDetail)
                .HasMaxLength(150)
                .IsRequired(false);

            configuration.HasIndex("ExtendableTypeName", "CustomAttributeType", "AttributeKey").IsUnique(true);
        }
    }
}
