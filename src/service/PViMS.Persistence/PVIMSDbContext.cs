using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using PVIMS.Core.Entities;
using PVIMS.Core.SeedWork;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace PVIMS.Persistence
{
    public class PVIMSDbContext : DbContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        //public DbSet<AuditLog> AuditLogs { get; set; }

        public PVIMSDbContext(DbContextOptions<PVIMSDbContext> options) : base(options) { }

        public PVIMSDbContext(DbContextOptions<PVIMSDbContext> options,
            IHttpContextAccessor httpContextAccessor) : base(options) 
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.ApplyConfiguration(new AuditLogEntityTypeConfiguration());
        }

        public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            foreach (var changedEntity in ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified))
            {
                if (typeof(AuditedEntity<int, User>).IsAssignableFrom(changedEntity.Entity.GetType()))
                {
                    var changedAuditedEntity = (AuditedEntity<int, User>)changedEntity.Entity;

                    try
                    {
                        User currentUser = null;

                        var userName = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

                        //if (Users != null)
                        //{
                        //    currentUser = _userContext != null ? Users.SingleOrDefault(u => u.UserName == userName) : null;
                        //}

                        changedAuditedEntity.AuditStamp(currentUser);
                    }
                    catch (Exception)
                    {
                        changedAuditedEntity.AuditStamp(null);
                    }
                }
            }

            var result = await base.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
