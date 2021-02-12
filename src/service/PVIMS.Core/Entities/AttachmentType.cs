using System.Collections.Generic;

namespace PVIMS.Core.Entities
{
    public partial class AttachmentType : EntityBase
    {
        public AttachmentType()
        {
            Attachments = new HashSet<Attachment>();
        }

        public string Description { get; set; }
        public string Key { get; set; }

        public virtual ICollection<Attachment> Attachments { get; set; }
    }
}
