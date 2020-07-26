using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PVIMS.Core.Entities
{
	[Table(nameof(WorkFlow))]
	public class WorkFlow : EntityBase
	{
		public WorkFlow()
		{
            WorkFlowGuid = Guid.NewGuid();

            Activities = new HashSet<Activity>();
		}

        [Required]
		[StringLength(100)]
		public string Description { get; set; }

        public Guid WorkFlowGuid { get; set; }

        public virtual ICollection<Activity> Activities { get; set; }
	}
}