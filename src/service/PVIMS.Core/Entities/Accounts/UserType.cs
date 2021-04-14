using System.Runtime.Serialization;

namespace PVIMS.Core.Entities.Accounts
{
    public enum UserType
    {
        [EnumMember(Value = "User")]
        User,
        [EnumMember(Value = "SystemAdmin")]
        SystemAdmin
    }
}
