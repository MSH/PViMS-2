using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PVIMS.Core.Entities
{
    [Table(nameof(PostDeployment))]
    public class PostDeployment : EntityBase
    {
        [Required]
        public Guid ScriptGuid { get; set; }
        [Required]
        [StringLength(200)]
        public string ScriptFileName { get; set; }
        [Required]
        [StringLength(200)]
        public string ScriptDescription { get; set; }
        public DateTime? RunDate { get; set; }
        public int? StatusCode { get; set; }
        public string StatusMessage { get; set; }
        [Required]
        public int RunRank { get; set; }
    }
}
