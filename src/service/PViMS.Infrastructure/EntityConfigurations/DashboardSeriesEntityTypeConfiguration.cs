using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PVIMS.Core.Aggregates.DashboardAggregate;

namespace PVIMS.Infrastructure.EntityConfigurations
{
    class DashboardSeriesEntityTypeConfiguration : IEntityTypeConfiguration<DashboardSeries>
    {
        public void Configure(EntityTypeBuilder<DashboardSeries> configuration)
        {
            configuration.ToTable("DashboardSeries");

            configuration.HasKey(e => e.Id);

            configuration.Ignore(b => b.DomainEvents);

            configuration.Property(e => e.Created)
                .IsRequired();

            configuration.Property(e => e.CreatedById)
                .IsRequired()
                .HasColumnName("CreatedBy_Id");

            configuration.Property(e => e.UpdatedById)
                .HasColumnName("UpdatedBy_Id");

            configuration.Property(b => b.Name)
                .HasMaxLength(100)
                .IsRequired();

            configuration.Property<int>("DashboardElementId")
                .IsRequired();

            configuration.HasOne(d => d.CreatedBy)
                .WithMany(p => p.DashboardSeriesCreations)
                .HasForeignKey(d => d.CreatedById);

            configuration.HasOne(d => d.UpdatedBy)
                .WithMany(p => p.DashboardSeriesUpdates)
                .HasForeignKey(d => d.UpdatedById);

            configuration.HasIndex(e => e.CreatedById);
            configuration.HasIndex(e => e.UpdatedById);
        }
    }
}
