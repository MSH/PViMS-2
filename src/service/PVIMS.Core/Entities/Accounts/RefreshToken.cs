using PVIMS.Core.SeedWork;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace PVIMS.Core.Entities.Accounts
{
    [Table(nameof(RefreshToken))]
    public class RefreshToken : Entity<int>
    {
        public RefreshToken() : base() { /* Required by EF */ }

        public RefreshToken(string token, DateTime expires, string remoteIpAddress) : this()
        {
            Token = token;
            Expires = expires;
            RemoteIpAddress = remoteIpAddress;
        }

        public string Token { get; set; }

        public DateTime Expires { get; set; }

        public User User { get; set; }

        public bool Active => DateTime.UtcNow <= Expires;

        public string RemoteIpAddress { get; set; }
    }

}
