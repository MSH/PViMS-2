using PVIMS.Core.Aggregates.ReportInstanceAggregate;
using System;
using System.Collections.Generic;

namespace PVIMS.Core.Entities
{
	public class WorkFlow : EntityBase
	{
		public WorkFlow()
		{
            WorkFlowGuid = Guid.NewGuid();

            Activities = new HashSet<Activity>();
			ReportInstances = new HashSet<ReportInstance>();
		}

		public string Description { get; set; }
		public Guid WorkFlowGuid { get; set; }

		public virtual ICollection<Activity> Activities { get; set; }
		public virtual ICollection<ReportInstance> ReportInstances { get; set; }
	}
}