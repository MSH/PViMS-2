using System.ComponentModel.DataAnnotations.Schema;

namespace PVIMS.Core.Entities
{
	[Table(nameof(PatientLanguage))]
	public class PatientLanguage : EntityBase
	{
		public bool Preferred { get; set; }
		public virtual Language Language { get; set; }
		public virtual Patient Patient { get; set; }
	}
}