using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PVIMS.Core.Entities;

namespace PVIMS.Infrastructure.EntityConfigurations
{
    class DatasetXmlAttributeEntityTypeConfiguration : IEntityTypeConfiguration<DatasetXmlAttribute>
    {
        public void Configure(EntityTypeBuilder<DatasetXmlAttribute> configuration)
        {
            configuration.ToTable("DatasetXmlAttribute");

            configuration.HasKey(e => e.Id);

            configuration.Property(c => c.AttributeName)
                .HasMaxLength(50)
                .IsRequired(true);

            configuration.Property(c => c.AttributeValue)
                .IsRequired(false);

            configuration.Property(c => c.Created)
                .IsRequired(true);

            configuration.Property(c => c.LastUpdated)
                .IsRequired(false);

            configuration.HasOne(p => p.CreatedBy)
                .WithMany()
                .HasForeignKey("CreatedBy_Id");

            configuration.HasOne(p => p.DatasetElement)
                .WithMany()
                .HasForeignKey("DatasetElement_Id");

            configuration.HasOne(p => p.ParentNode)
                .WithMany()
                .HasForeignKey("ParentNode_Id")
                .IsRequired(true);

            configuration.HasOne(p => p.UpdatedBy)
                .WithMany()
                .HasForeignKey("UpdatedBy_Id");
        }
    }
}
