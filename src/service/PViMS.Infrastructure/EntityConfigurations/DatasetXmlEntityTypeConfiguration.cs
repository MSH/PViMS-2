using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PVIMS.Core.Entities;

namespace PVIMS.Infrastructure.EntityConfigurations
{
    class DatasetXmlEntityTypeConfiguration : IEntityTypeConfiguration<DatasetXml>
    {
        public void Configure(EntityTypeBuilder<DatasetXml> configuration)
        {
            configuration.ToTable("DatasetXml");

            configuration.HasKey(e => e.Id);

            configuration.Property(c => c.Description)
                .HasMaxLength(50)
                .IsRequired(true);

            configuration.Property(c => c.Created)
                .IsRequired(true);

            configuration.Property(c => c.LastUpdated)
                .IsRequired(false);

            configuration.HasOne(p => p.CreatedBy)
                .WithMany()
                .HasForeignKey("CreatedBy_Id");

            configuration.HasOne(p => p.UpdatedBy)
                .WithMany()
                .HasForeignKey("UpdatedBy_Id");

            configuration.HasMany(c => c.ChildrenNodes)
               .WithOne()
               .HasForeignKey("DatasetXml_Id")
               .IsRequired(true)
               .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
