﻿using Microsoft.EntityFrameworkCore;
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
                .IsRequired()
                .HasMaxLength(10);

            configuration.Property(c => c.FacilityName)
                .IsRequired()
                .HasMaxLength(100);

            configuration.Property(e => e.FacilityTypeId)
                .IsRequired()
                .HasColumnName("FacilityType_Id");

            configuration.Property(c => c.FaxNumber)
                .HasMaxLength(30);

            configuration.Property(c => c.MobileNumber)
                .HasMaxLength(30);

            configuration.Property(e => e.OrgUnitId)
                .HasColumnName("OrgUnit_Id");

            configuration.Property(c => c.TelNumber)
                .HasMaxLength(30);

            configuration.HasOne(d => d.FacilityType)
                .WithMany(p => p.Facilities)
                .HasForeignKey(d => d.FacilityTypeId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_dbo.Facility_dbo.FacilityType_FacilityType_Id");

            configuration.HasOne(d => d.OrgUnit)
                .WithMany(p => p.Facilities)
                .HasForeignKey(d => d.OrgUnitId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_dbo.Facility_dbo.OrgUnit_OrgUnit_Id");

            configuration.HasIndex("FacilityCode").IsUnique(true);
            configuration.HasIndex("FacilityName").IsUnique(true);
            configuration.HasIndex(e => e.FacilityTypeId, "IX_FacilityType_Id");
            configuration.HasIndex(e => e.OrgUnitId, "IX_OrgUnit_Id");
        }
    }
}