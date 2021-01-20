using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PVIMS.Core.Entities;

namespace PVIMS.Infrastructure.EntityConfigurations
{
    class ConditionEntityTypeConfiguration : IEntityTypeConfiguration<Condition>
    {
        public void Configure(EntityTypeBuilder<Condition> configuration)
        {
            configuration.ToTable("Condition");

            configuration.HasKey(e => e.Id);

            configuration.Property(c => c.Description)
                .HasMaxLength(50)
                .IsRequired(true);

            configuration.Property(c => c.Chronic)
                .HasDefaultValue(false)
                .IsRequired(true);

            configuration.Property(c => c.Active)
                .HasDefaultValue(true)
                .IsRequired(true);

            configuration.HasMany(c => c.ConditionLabTests)
               .WithOne()
               .HasForeignKey("Condition_Id")
               .IsRequired(true)
               .OnDelete(DeleteBehavior.Cascade);

            configuration.HasMany(c => c.ConditionMedications)
               .WithOne()
               .HasForeignKey("Condition_Id")
               .IsRequired(true)
               .OnDelete(DeleteBehavior.Cascade);

            configuration.HasMany(c => c.ConditionMedDras)
               .WithOne()
               .HasForeignKey("Condition_Id")
               .IsRequired(true)
               .OnDelete(DeleteBehavior.Cascade);

            configuration.HasIndex("Description").IsUnique(true);
        }
    }
}
