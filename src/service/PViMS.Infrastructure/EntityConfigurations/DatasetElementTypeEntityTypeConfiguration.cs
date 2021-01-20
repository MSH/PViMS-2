using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PVIMS.Core.Entities;

namespace PVIMS.Infrastructure.EntityConfigurations
{
    class DatasetElementTypeEntityTypeConfiguration : IEntityTypeConfiguration<DatasetElementType>
    {
        public void Configure(EntityTypeBuilder<DatasetElementType> configuration)
        {
            configuration.ToTable("DatasetElementType");

            configuration.HasKey(e => e.Id);

            configuration.Property(c => c.Description)
                .HasMaxLength(50)
                .IsRequired(true);

            configuration.HasIndex("Description").IsUnique(true);
        }
    }
}
