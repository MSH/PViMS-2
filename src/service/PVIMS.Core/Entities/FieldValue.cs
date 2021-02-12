namespace PVIMS.Core.Entities
{
	public class FieldValue : EntityBase
	{
		public string Value { get; set; }
		public bool Default { get; set; }
		public bool Other { get; set; }
		public bool Unknown { get; set; }
		public int FieldId { get; set; }

		public virtual Field Field { get; set; }
	}
}