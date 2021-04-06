﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PVIMS.Core.Entities;

namespace PVIMS.Infrastructure.EntityConfigurations
{
    class FieldValueEntityTypeConfiguration : IEntityTypeConfiguration<FieldValue>
    {
        public void Configure(EntityTypeBuilder<FieldValue> configuration)
        {
            configuration.ToTable("FieldValue");

            configuration.HasKey(e => e.Id);

            configuration.Property(e => e.Value)
                .IsRequired()
                .HasMaxLength(100);

            configuration.Property(e => e.FieldId)
                .IsRequired()
                .HasColumnName("Field_Id");

            configuration.Property(c => c.Default)
                .IsRequired()
                .HasDefaultValue(true);

            configuration.Property(c => c.Other)
                .IsRequired()
                .HasDefaultValue(false);

            configuration.Property(c => c.Unknown)
                .IsRequired()
                .HasDefaultValue(false);

            configuration.HasOne(d => d.Field)
                .WithMany(p => p.FieldValues)
                .HasForeignKey(d => d.FieldId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_dbo.FieldValue_dbo.Field_Field_Id");

            configuration.HasIndex(e => e.FieldId, "IX_Field_Id");
        }
    }
}