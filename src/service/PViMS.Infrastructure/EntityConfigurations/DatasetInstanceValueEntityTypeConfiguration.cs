using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PVIMS.Core.Entities;

namespace PVIMS.Infrastructure.EntityConfigurations
{
    class DatasetInstanceValueEntityTypeConfiguration : IEntityTypeConfiguration<DatasetInstanceValue>
    {
        public void Configure(EntityTypeBuilder<DatasetInstanceValue> configuration)
        {
            configuration.ToTable("DatasetInstanceValue");

            configuration.HasKey(e => e.Id);

            configuration.Property(c => c.InstanceValue)
                .IsRequired(true);

            configuration.HasOne(c => c.DatasetElement)
                .WithMany()
                .HasForeignKey("DatasetElement_Id")
                .IsRequired(true);

            configuration.HasOne(c => c.DatasetInstance)
                .WithMany()
                .HasForeignKey("DatasetInstance_Id")
                .IsRequired(true);

            configuration.HasIndex("DatasetInstance_Id", "DatasetElement_Id").IsUnique(true);
        }
    }
}
