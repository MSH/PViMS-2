using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PVIMS.Core.Aggregates.DashboardAggregate;

namespace PVIMS.Infrastructure.EntityConfigurations
{
    class DashboardElementEntityTypeConfiguration : IEntityTypeConfiguration<DashboardElement>
    {
        public void Configure(EntityTypeBuilder<DashboardElement> configuration)
        {
            configuration.ToTable("DashboardElement");

            configuration.HasKey(e => e.Id);

            configuration.Ignore(b => b.DomainEvents);

            configuration.Property(e => e.Created)
                .IsRequired();

            configuration.Property(e => e.CreatedById)
                .IsRequired()
                .HasColumnName("CreatedBy_Id");

            configuration.Property(e => e.UpdatedById)
                .HasColumnName("UpdatedBy_Id");

            configuration.Property<int>("DashboardId")
                .IsRequired();

            configuration.Property(b => b.Name)
                .HasMaxLength(50)
                .IsRequired();

            configuration.Property(b => b.ShortName)
                .HasMaxLength(20);

            configuration
                .Property(c => c.Active)
                .IsRequired();

            configuration.HasOne(d => d.CreatedBy)
                .WithMany(p => p.DashboardElementCreations)
                .HasForeignKey(d => d.CreatedById);

            configuration.HasOne(d => d.UpdatedBy)
                .WithMany(p => p.DashboardElementUpdates)
                .HasForeignKey(d => d.UpdatedById);

            configuration.HasMany(b => b.SeriesElements)
               .WithOne()
               .HasForeignKey("DashboardElementId")
               .OnDelete(DeleteBehavior.Cascade);

            configuration.HasMany(b => b.VisualisationElements)
               .WithOne()
               .HasForeignKey("DashboardElementId")
               .OnDelete(DeleteBehavior.Cascade);

            configuration.HasIndex(e => e.CreatedById);
            configuration.HasIndex(e => e.UpdatedById);
        }
    }
}
