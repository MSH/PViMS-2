using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PVIMS.Core.Entities;
using PVIMS.Core.ValueTypes;

namespace PVIMS.Infrastructure.EntityConfigurations
{
    class DatasetXmlNodeEntityTypeConfiguration : IEntityTypeConfiguration<DatasetXmlNode>
    {
        public void Configure(EntityTypeBuilder<DatasetXmlNode> configuration)
        {
            configuration.ToTable("DatasetXmlNode");

            configuration.HasKey(e => e.Id);

            configuration.Property(c => c.NodeName)
                .HasMaxLength(50)
                .IsRequired(true);

            configuration.Property(c => c.NodeType)
                .HasConversion(x => (int)x, x => (NodeType)x);

            configuration.Property(c => c.NodeValue)
                .IsRequired(false);

            configuration.Property(c => c.Created)
                .IsRequired(true);

            configuration.Property(c => c.LastUpdated)
                .IsRequired(false);

            configuration.HasOne(p => p.ParentNode)
                .WithMany()
                .HasForeignKey("ParentNode_Id");

            configuration.HasOne(p => p.CreatedBy)
                .WithMany()
                .HasForeignKey("CreatedBy_Id");

            configuration.HasOne(p => p.DatasetElement)
                .WithMany()
                .HasForeignKey("DatasetElement_Id");

            configuration.HasOne(p => p.UpdatedBy)
                .WithMany()
                .HasForeignKey("UpdatedBy_Id");

            configuration.HasOne(p => p.DatasetElementSub)
                .WithMany()
                .HasForeignKey("DatasetElementSub_Id");

            configuration.HasMany(c => c.ChildrenNodes)
               .WithOne()
               .HasForeignKey("ParentNode_Id")
               .IsRequired(false)
               .OnDelete(DeleteBehavior.Cascade);

            configuration.HasMany(c => c.NodeAttributes)
               .WithOne()
               .HasForeignKey("ParentNode_Id")
               .IsRequired(true)
               .OnDelete(DeleteBehavior.Cascade);

            configuration.HasIndex("DatasetXml_Id", "NodeName").IsUnique(true);
        }
    }
}
