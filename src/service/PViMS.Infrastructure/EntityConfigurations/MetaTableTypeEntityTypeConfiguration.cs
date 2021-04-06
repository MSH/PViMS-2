﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PVIMS.Core.Entities;

namespace PVIMS.Infrastructure.EntityConfigurations
{
    class MetaTableTypeEntityTypeConfiguration : IEntityTypeConfiguration<MetaTableType>
    {
        public void Configure(EntityTypeBuilder<MetaTableType> configuration)
        {
            configuration.ToTable("MetaTableType");

            configuration.HasKey(e => e.Id);

            configuration.Property(e => e.Description)
                .IsRequired()
                .HasMaxLength(50);

            configuration.Property(e => e.MetaTableTypeGuid)
                .IsRequired()
                .HasColumnName("metatabletype_guid");

            configuration.HasIndex("Description").IsUnique(true);
        }
    }
}
