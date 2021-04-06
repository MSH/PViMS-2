using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PVIMS.Core.Entities;

namespace PVIMS.Infrastructure.EntityConfigurations
{
    class MetaColumnEntityTypeConfiguration : IEntityTypeConfiguration<MetaColumn>
    {
        public void Configure(EntityTypeBuilder<MetaColumn> configuration)
        {
            configuration.ToTable("MetaColumn");

            configuration.HasKey(e => e.Id);

            configuration.Property(e => e.ColumnName)
                .IsRequired()
                .HasMaxLength(100);

            configuration.Property(e => e.ColumnTypeId)
                .IsRequired()
                .HasColumnName("ColumnType_Id");

            configuration.Property(e => e.MetaColumnGuid)
                .IsRequired()
                .HasColumnName("metacolumn_guid");

            configuration.Property(e => e.TableId)
                .IsRequired()
                .HasColumnName("Table_Id");

            configuration.HasOne(d => d.ColumnType)
                .WithMany(p => p.MetaColumns)
                .HasForeignKey(d => d.ColumnTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_dbo.MetaColumn_dbo.MetaColumnType_ColumnType_Id");

            configuration.HasOne(d => d.Table)
                .WithMany(p => p.Columns)
                .HasForeignKey(d => d.TableId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_dbo.MetaColumn_dbo.MetaTable_Table_Id");

            configuration.HasIndex(e => e.ColumnTypeId, "IX_ColumnType_Id");
            configuration.HasIndex(e => e.TableId, "IX_Table_Id");
        }
    }
}
