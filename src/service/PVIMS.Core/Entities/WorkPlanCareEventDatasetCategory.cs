using System.ComponentModel.DataAnnotations.Schema;

namespace PVIMS.Core.Entities
{
	[Table(nameof(WorkPlanCareEventDatasetCategory))]
	public class WorkPlanCareEventDatasetCategory : EntityBase
	{
		public virtual DatasetCategory DatasetCategory { get; set; }
		public virtual WorkPlanCareEvent WorkPlanCareEvent { get; set; }
	}
}