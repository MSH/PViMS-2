using System.ComponentModel.DataAnnotations.Schema;
using VPS.Common.Domain;

namespace PVIMS.Core.Entities
{
    [Table(nameof(UserRole))]
    public class UserRole : Entity<int>
    {
        public User User { get; set; }
        public Role Role { get; set; }
    }
}
