using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PVIMS.Core.Entities;

namespace PVIMS.Infrastructure.EntityConfigurations
{
    class DatasetInstanceSubValueEntityTypeConfiguration : IEntityTypeConfiguration<DatasetInstanceSubValue>
    {
        public void Configure(EntityTypeBuilder<DatasetInstanceSubValue> configuration)
        {
            configuration.ToTable("DatasetInstanceSubValue");

            configuration.HasKey(e => e.Id);

            configuration.Property(e => e.DatasetElementSubId)
                .IsRequired()
                .HasColumnName("DatasetElementSub_Id");

            configuration.Property(e => e.DatasetInstanceValueId)
                .IsRequired()
                .HasColumnName("DatasetInstanceValue_Id");

            configuration.Property(c => c.ContextValue)
                .IsRequired();

            configuration.Property(c => c.InstanceValue)
                .IsRequired();

            configuration.HasOne(d => d.DatasetElementSub)
                .WithMany(p => p.DatasetInstanceSubValues)
                .HasForeignKey(d => d.DatasetElementSubId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_dbo.DatasetInstanceSubValue_dbo.DatasetElementSub_DatasetElementSub_Id");

            configuration.HasOne(d => d.DatasetInstanceValue)
                .WithMany(p => p.DatasetInstanceSubValues)
                .HasForeignKey(d => d.DatasetInstanceValueId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_dbo.DatasetInstanceSubValue_dbo.DatasetInstanceValue_DatasetInstanceValue_Id");

            configuration.HasIndex(e => new { e.DatasetInstanceValueId, e.DatasetElementSubId }).IsUnique(true);
            configuration.HasIndex(e => e.DatasetElementSubId, "IX_DatasetElementSub_Id");
            configuration.HasIndex(e => e.DatasetInstanceValueId, "IX_DatasetInstanceValue_Id");
        }
    }
}
