using PVIMS.Core.SeedWork;
using System.Collections.Generic;

namespace PVIMS.Core.Entities.Accounts
{
    public class Role : Entity<int>
    {
        public Role()
        {
            UserRoles = new HashSet<UserRole>();
        }

        public string Name { get; set; }
        public string Key { get; set; }

        public virtual ICollection<UserRole> UserRoles { get; set; }
    }
}
