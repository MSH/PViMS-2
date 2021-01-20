using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PVIMS.Core.Entities;

namespace PVIMS.Infrastructure.EntityConfigurations
{
    class FacilityEntityTypeConfiguration : IEntityTypeConfiguration<Facility>
    {
        public void Configure(EntityTypeBuilder<Facility> configuration)
        {
            configuration.ToTable("Facility");

            configuration.HasKey(e => e.Id);

            configuration.Property(c => c.FacilityCode)
                .HasMaxLength(10)
                .IsRequired(true);

            configuration.Property(c => c.FacilityName)
                .HasMaxLength(100)
                .IsRequired(true);

            configuration.HasOne(c => c.FacilityType)
                .WithMany()
                .HasForeignKey("FacilityType_Id")
                .IsRequired(true);

            configuration.Property(c => c.TelNumber)
                .HasMaxLength(30)
                .IsRequired(false);

            configuration.Property(c => c.MobileNumber)
                .HasMaxLength(30)
                .IsRequired(false);

            configuration.Property(c => c.FaxNumber)
                .HasMaxLength(30)
                .IsRequired(false);

            configuration.HasOne(c => c.OrgUnit)
                .WithMany()
                .HasForeignKey("OrgUnit_Id")
                .IsRequired(false);

            configuration.HasIndex("FacilityCode").IsUnique(true);
            configuration.HasIndex("FacilityName").IsUnique(true);
        }
    }
}
