using System.ComponentModel.DataAnnotations.Schema;

namespace PVIMS.Core.Entities
{
    [Table(nameof(UserFacility))]
    public class UserFacility : EntityBase
    {
        public User User { get; set; }
        public Facility Facility { get; set; }
    }
}
