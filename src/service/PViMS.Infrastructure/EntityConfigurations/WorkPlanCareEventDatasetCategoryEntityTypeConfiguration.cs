﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PVIMS.Core.Entities;

namespace PVIMS.Infrastructure.EntityConfigurations
{
    class WorkPlanCareEventDatasetCategoryEntityTypeConfiguration : IEntityTypeConfiguration<WorkPlanCareEventDatasetCategory>
    {
        public void Configure(EntityTypeBuilder<WorkPlanCareEventDatasetCategory> configuration)
        {
            configuration.ToTable("WorkPlanCareEventDatasetCategory");

            configuration.HasKey(e => e.Id);

            configuration.Property(e => e.DatasetCategoryId)
                .IsRequired()
                .HasColumnName("DatasetCategory_Id");

            configuration.Property(e => e.WorkPlanCareEventId)
                .IsRequired()
                .HasColumnName("WorkPlanCareEvent_Id");

            configuration.HasOne(d => d.DatasetCategory)
                .WithMany(p => p.WorkPlanCareEventDatasetCategories)
                .HasForeignKey(d => d.DatasetCategoryId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_dbo.WorkPlanCareEventDatasetCategory_dbo.DatasetCategory_DatasetCategory_Id");

            configuration.HasOne(d => d.WorkPlanCareEvent)
                .WithMany(p => p.WorkPlanCareEventDatasetCategories)
                .HasForeignKey(d => d.WorkPlanCareEventId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_dbo.WorkPlanCareEventDatasetCategory_dbo.WorkPlanCareEvent_WorkPlanCareEvent_Id");

            configuration.HasIndex(e => new { e.WorkPlanCareEventId, e.DatasetCategoryId }).IsUnique(true);
            configuration.HasIndex(e => e.DatasetCategoryId, "IX_DatasetCategory_Id");
            configuration.HasIndex(e => e.WorkPlanCareEventId, "IX_WorkPlanCareEvent_Id");
        }
    }
}
