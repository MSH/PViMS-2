using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PVIMS.Core.Entities;

namespace PVIMS.Infrastructure.EntityConfigurations
{
    class DatasetElementEntityTypeConfiguration : IEntityTypeConfiguration<DatasetElement>
    {
        public void Configure(EntityTypeBuilder<DatasetElement> configuration)
        {
            configuration.ToTable("DatasetElement");

            configuration.HasKey(e => e.Id);

            configuration.Property(c => c.ElementName)
                .HasMaxLength(100)
                .IsRequired(true);

            configuration.HasOne(c => c.Field)
                .WithMany()
                .HasForeignKey("Field_Id");

            configuration.HasOne(c => c.DatasetElementType)
                .WithMany()
                .HasForeignKey("DatasetElementType_Id");

            configuration.Property(c => c.OID)
                .HasMaxLength(50)
                .IsRequired(false);

            configuration.Property(c => c.DefaultValue)
                .IsRequired(false);

            configuration.Property(c => c.System)
                .HasDefaultValue(true)
                .IsRequired(true);

            configuration.Property(c => c.UID)
                .HasMaxLength(10)
                .IsRequired(false);

            configuration.Property(c => c.DatasetElementGuid)
                .IsRequired(true)
                .HasDefaultValueSql("newid()");

            configuration.HasMany(c => c.DatasetCategoryElements)
               .WithOne()
               .HasForeignKey("DatasetElement_Id")
               .IsRequired(true)
               .OnDelete(DeleteBehavior.Cascade);

            configuration.HasMany(c => c.DatasetElementSubs)
               .WithOne()
               .HasForeignKey("DatasetElement_Id")
               .IsRequired(true)
               .OnDelete(DeleteBehavior.Cascade);

            configuration.HasMany(c => c.DatasetRules)
               .WithOne()
               .HasForeignKey("DatasetElement_Id")
               .IsRequired(true)
               .OnDelete(DeleteBehavior.Cascade);

            configuration.HasIndex("ElementName").IsUnique(true);
        }
    }
}
