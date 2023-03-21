using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PVIMS.Core.Aggregates.DashboardAggregate;

namespace PVIMS.Infrastructure.EntityConfigurations
{
    class DashboardEntityTypeConfiguration : IEntityTypeConfiguration<Dashboard>
    {
        public void Configure(EntityTypeBuilder<Dashboard> configuration)
        {
            configuration.ToTable("Dashboard");

            configuration.HasKey(e => e.Id);

            configuration.Ignore(b => b.DomainEvents);

            configuration.Property(e => e.Created)
                .IsRequired();

            configuration.Property(e => e.CreatedById)
                .IsRequired()
                .HasColumnName("CreatedBy_Id");

            configuration.Property(e => e.UpdatedById)
                .HasColumnName("UpdatedBy_Id");

            configuration.Property(b => b.UID)
                .HasMaxLength(20)
                .IsRequired();

            configuration.Property(b => b.Name)
                .HasMaxLength(50)
                .IsRequired();

            configuration.Property(b => b.ShortName)
                .HasMaxLength(20);

            configuration.Property(b => b.Icon)
                .HasMaxLength(50);

            configuration
                .Property(c => c.FrequencyId)
                .IsRequired();

            configuration
                .Property(c => c.Active)
                .IsRequired();

            configuration.HasOne(d => d.CreatedBy)
                .WithMany(p => p.DashboardCreations)
                .HasForeignKey(d => d.CreatedById);

            configuration.HasOne(d => d.UpdatedBy)
                .WithMany(p => p.DashboardUpdates)
                .HasForeignKey(d => d.UpdatedById);

            configuration.HasMany(b => b.Elements)
               .WithOne()
               .HasForeignKey("DashboardId")
               .OnDelete(DeleteBehavior.Cascade);

            configuration.HasIndex(e => e.CreatedById);
            configuration.HasIndex(e => e.UpdatedById);
        }
    }
}
