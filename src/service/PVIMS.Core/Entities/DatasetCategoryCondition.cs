﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PVIMS.Core.Entities
{
    [Table(nameof(DatasetCategoryCondition))]
    public class DatasetCategoryCondition : EntityBase
    {
        [Required]
        public virtual DatasetCategory DatasetCategory { get; set; }
        [Required]
        public virtual Condition Condition { get; set; }
    }
}