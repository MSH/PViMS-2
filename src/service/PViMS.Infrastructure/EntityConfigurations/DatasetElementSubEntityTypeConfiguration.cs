using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PVIMS.Core.Entities;

namespace PVIMS.Infrastructure.EntityConfigurations
{
    class DatasetElementSubEntityTypeConfiguration : IEntityTypeConfiguration<DatasetElementSub>
    {
        public void Configure(EntityTypeBuilder<DatasetElementSub> configuration)
        {
            configuration.ToTable("DatasetElementSub");

            configuration.HasKey(e => e.Id);

            configuration.Property(c => c.ElementName)
                .HasMaxLength(100)
                .IsRequired(true);

            configuration.Property(c => c.FieldOrder)
                .IsRequired(true);

            configuration.HasOne(c => c.DatasetElement)
                .WithMany()
                .HasForeignKey("DatasetElement_Id")
                .IsRequired(true);

            configuration.HasOne(c => c.Field)
                .WithMany()
                .HasForeignKey("Field_Id");

            configuration.Property(c => c.OID)
                .HasMaxLength(50)
                .IsRequired(false);

            configuration.Property(c => c.DefaultValue)
                .IsRequired(false);

            configuration.Property(c => c.System)
                .HasDefaultValue(true)
                .IsRequired(true);

            configuration.Property(c => c.FriendlyName)
                .HasMaxLength(150)
                .IsRequired(false);

            configuration.Property(c => c.Help)
                .HasMaxLength(350)
                .IsRequired(false);

            configuration.HasIndex("DatasetElement_Id", "ElementName").IsUnique(true);
        }
    }
}
