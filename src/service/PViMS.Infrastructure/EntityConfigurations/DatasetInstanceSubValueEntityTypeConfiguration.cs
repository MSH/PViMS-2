using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PVIMS.Core.Entities;

namespace PVIMS.Infrastructure.EntityConfigurations
{
    class DatasetInstanceSubValueEntityTypeConfiguration : IEntityTypeConfiguration<DatasetInstanceSubValue>
    {
        public void Configure(EntityTypeBuilder<DatasetInstanceSubValue> configuration)
        {
            configuration.ToTable("DatasetInstanceSubValue");

            configuration.HasKey(e => e.Id);

            configuration.Property(c => c.ContextValue)
                .IsRequired(true);

            configuration.Property(c => c.InstanceValue)
                .IsRequired(true);

            configuration.HasOne(c => c.DatasetElementSub)
                .WithMany()
                .HasForeignKey("DatasetElementSub_Id")
                .IsRequired(true);

            configuration.HasOne(c => c.DatasetInstanceValue)
                .WithMany()
                .HasForeignKey("DatasetInstanceValue_Id")
                .IsRequired(true);

            configuration.HasIndex("DatasetInstanceValue_Id", "DatasetElementSub_Id").IsUnique(true);
        }
    }
}
