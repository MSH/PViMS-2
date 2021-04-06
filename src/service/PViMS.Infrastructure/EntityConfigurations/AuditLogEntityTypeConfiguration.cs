using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PVIMS.Core.Entities;
using PVIMS.Core.ValueTypes;

namespace PVIMS.Infrastructure.EntityConfigurations
{
    class AuditLogEntityTypeConfiguration : IEntityTypeConfiguration<AuditLog>
    {
        public void Configure(EntityTypeBuilder<AuditLog> configuration)
        {
            configuration.ToTable("AuditLog");

            configuration.HasKey(e => e.Id);

            configuration.Property(c => c.ActionDate)
                .IsRequired()
                .HasColumnType("datetime");

            configuration.Property(c => c.AuditType)
                .HasConversion(x => (int)x, x => (AuditType)x);

            configuration.Property(e => e.UserId)
                .IsRequired()
                .HasColumnName("User_Id");

            configuration.HasOne(d => d.User)
                .WithMany(p => p.AuditLogs)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_dbo.AuditLog_dbo.User_User_Id");

            configuration.HasIndex("ActionDate").IsUnique(false);
            configuration.HasIndex(e => e.UserId, "IX_User_Id");
        }
    }
}
