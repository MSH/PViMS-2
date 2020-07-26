using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PVIMS.Core.Entities
{
	[Table(nameof(DatasetElementType))]
	public class DatasetElementType : EntityBase
	{
		public DatasetElementType()
		{
			DatasetElements = new HashSet<DatasetElement>();
		}

		[Required]
		[StringLength(50)]
		public string Description { get; set; }

		public virtual ICollection<DatasetElement> DatasetElements { get; set; }
	}
}