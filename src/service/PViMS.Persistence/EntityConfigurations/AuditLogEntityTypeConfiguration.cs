using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PVIMS.Core.Entities;

namespace PVIMS.Infrastructure.EntityConfigurations
{
    class AuditLogEntityTypeConfiguration : IEntityTypeConfiguration<AuditLog>
    {
        public void Configure(EntityTypeBuilder<AuditLog> configuration)
        {
            configuration.ToTable("AuditLog");

            configuration.HasKey(e => e.Id);

            configuration.Property<int>("AuditType")
                .IsRequired();

            configuration.Property(c => c.ActionDate)
                .IsRequired(true);

            configuration.Property(c => c.Details)
                .IsRequired(false);

            configuration.HasOne(c => c.User)
                .WithMany()
                .HasForeignKey("User_Id");

            configuration.Property(c => c.Log)
                .IsRequired(false);

            configuration.HasIndex("ActionDate").IsUnique(false);
        }
    }
}
