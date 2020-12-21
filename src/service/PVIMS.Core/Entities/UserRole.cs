using PVIMS.Core.SeedWork;
using System.ComponentModel.DataAnnotations.Schema;

namespace PVIMS.Core.Entities
{
    [Table(nameof(UserRole))]
    public class UserRole : Entity<int>
    {
        public User User { get; set; }
        public Role Role { get; set; }
    }
}
