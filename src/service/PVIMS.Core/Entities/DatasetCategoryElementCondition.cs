﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PVIMS.Core.Entities
{
    [Table(nameof(DatasetCategoryElementCondition))]
    public class DatasetCategoryElementCondition : EntityBase
    {
        [Required]
        public virtual DatasetCategoryElement DatasetCategoryElement { get; set; }
        [Required]
        public virtual Condition Condition { get; set; }
    }
}