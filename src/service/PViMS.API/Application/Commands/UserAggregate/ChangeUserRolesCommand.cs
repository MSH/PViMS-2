using MediatR;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace PVIMS.API.Application.Commands.UserAggregate
{
    [DataContract]
    public class ChangeUserRolesCommand
        : IRequest<bool>
    {
        [DataMember]
        public int UserId { get; private set; }

        [DataMember]
        public List<string> Roles { get; set; }

        public ChangeUserRolesCommand()
        {
        }

        public ChangeUserRolesCommand(int userId, List<string> roles) : this()
        {
            UserId = userId;
            Roles = roles;
        }
    }
}
