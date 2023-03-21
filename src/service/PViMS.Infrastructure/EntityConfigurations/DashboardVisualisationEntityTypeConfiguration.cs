using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PVIMS.Core.Aggregates.DashboardAggregate;

namespace PVIMS.Infrastructure.EntityConfigurations
{
    class DashboardVisualisationEntityTypeConfiguration : IEntityTypeConfiguration<DashboardVisualisation>
    {
        public void Configure(EntityTypeBuilder<DashboardVisualisation> configuration)
        {
            configuration.ToTable("DashboardVisualisation");

            configuration.HasKey(e => e.Id);

            configuration.Ignore(b => b.DomainEvents);

            configuration.Property(e => e.Created)
                .IsRequired();

            configuration.Property(e => e.CreatedById)
                .IsRequired()
                .HasColumnName("CreatedBy_Id");

            configuration.Property(e => e.UpdatedById)
                .HasColumnName("UpdatedBy_Id");

            configuration.Property<int>("DashboardElementId")
                .IsRequired();

            configuration
                .Property(c => c.ChartTypeId)
                .IsRequired();

            configuration.HasOne(d => d.CreatedBy)
                .WithMany(p => p.DashboardVisualisationCreations)
                .HasForeignKey(d => d.CreatedById);

            configuration.HasOne(d => d.UpdatedBy)
                .WithMany(p => p.DashboardVisualisationUpdates)
                .HasForeignKey(d => d.UpdatedById);

            configuration.HasIndex(e => e.CreatedById);
            configuration.HasIndex(e => e.UpdatedById);
        }
    }
}
