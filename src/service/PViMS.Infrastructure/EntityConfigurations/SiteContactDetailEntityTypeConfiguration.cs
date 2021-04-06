using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PVIMS.Core.Entities;

namespace PVIMS.Infrastructure.EntityConfigurations
{
    class SiteContactDetailEntityTypeConfiguration : IEntityTypeConfiguration<SiteContactDetail>
    {
        public void Configure(EntityTypeBuilder<SiteContactDetail> configuration)
        {
            configuration.ToTable("SiteContactDetail");

            configuration.HasKey(e => e.Id);

            configuration.Property(e => e.City)
                .IsRequired()
                .HasMaxLength(50);

            configuration.Property(e => e.ContactEmail)
                .HasMaxLength(50);

            configuration.Property(e => e.ContactFirstName)
                .IsRequired()
                .HasMaxLength(30);

            configuration.Property(e => e.ContactNumber)
                .HasMaxLength(50);

            configuration.Property(e => e.ContactSurname)
                .IsRequired()
                .HasMaxLength(30);

            configuration.Property(e => e.CountryCode)
                .HasMaxLength(10);

            configuration.Property(e => e.Created)
                .IsRequired()
                .HasColumnType("datetime");

            configuration.Property(e => e.CreatedById)
                .IsRequired()
                .HasColumnName("CreatedBy_Id");

            configuration.Property(e => e.LastUpdated)
                .HasColumnType("datetime");

            configuration.Property(e => e.OrganisationName)
                .IsRequired()
                .HasMaxLength(50);

            configuration.Property(e => e.PostCode)
                .HasMaxLength(20);

            configuration.Property(e => e.State)
                .HasMaxLength(50);

            configuration.Property(e => e.StreetAddress)
                .IsRequired()
                .HasMaxLength(100);

            configuration.Property(e => e.UpdatedById)
                .HasColumnName("UpdatedBy_Id");

            configuration.HasOne(d => d.CreatedBy)
                .WithMany(p => p.SiteContactDetailCreations)
                .HasForeignKey(d => d.CreatedById)
                .HasConstraintName("FK_dbo.SiteContactDetail_dbo.User_CreatedBy_Id");

            configuration.HasOne(d => d.UpdatedBy)
                .WithMany(p => p.SiteContactDetailUpdates)
                .HasForeignKey(d => d.UpdatedById)
                .HasConstraintName("FK_dbo.SiteContactDetail_dbo.User_UpdatedBy_Id");

            configuration.HasIndex(e => e.CreatedById, "IX_CreatedBy_Id");
            configuration.HasIndex(e => e.UpdatedById, "IX_UpdatedBy_Id");
        }
    }
}
