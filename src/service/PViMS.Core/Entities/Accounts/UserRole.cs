using PVIMS.Core.SeedWork;

namespace PVIMS.Core.Entities.Accounts
{
    public class UserRole : Entity<int>
    {
        public int RoleId { get; set; }
        public int UserId { get; set; }

        public User User { get; set; }
        public Role Role { get; set; }
    }
}
