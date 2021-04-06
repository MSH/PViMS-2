using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PVIMS.Core.Entities;

namespace PVIMS.Infrastructure.EntityConfigurations
{
    class MedDRAScaleEntityTypeConfiguration : IEntityTypeConfiguration<MedDRAScale>
    {
        public void Configure(EntityTypeBuilder<MedDRAScale> configuration)
        {
            configuration.ToTable("MedDRAScale");

            configuration.HasKey(e => e.Id);

            configuration.Property(e => e.GradingScaleId)
                .IsRequired()
                .HasColumnName("GradingScale_Id");

            configuration.Property(e => e.TerminologyMedDraId)
                .IsRequired()
                .HasColumnName("TerminologyMedDra_Id");

            configuration.HasOne(d => d.GradingScale)
                .WithMany(p => p.MedDrascales)
                .HasForeignKey(d => d.GradingScaleId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_dbo.MedDRAScale_dbo.SelectionDataItem_GradingScale_Id");

            configuration.HasOne(d => d.TerminologyMedDra)
                .WithMany(p => p.Scales)
                .HasForeignKey(d => d.TerminologyMedDraId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_dbo.MedDRAScale_dbo.TerminologyMedDra_TerminologyMedDra_Id");

            configuration.HasIndex(e => e.GradingScaleId, "IX_GradingScale_Id");
            configuration.HasIndex(e => e.TerminologyMedDraId, "IX_TerminologyMedDra_Id");
        }
    }
}
