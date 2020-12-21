using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using PVIMS.Infrastructure.EntityConfigurations;
using PVIMS.Core.Entities;
using PVIMS.Core.SeedWork;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace PVIMS.Infrastructure
{
    public class PVIMSDbContext : DbContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        // Database entities
        public DbSet<Activity> Activities { get; set; }
        public DbSet<ActivityExecutionStatus> ActivityExecutionStatuses { get; set; }
        public DbSet<ActivityExecutionStatusEvent> ActivityExecutionStatusEvents { get; set; }
        public DbSet<ActivityInstance> ActivityInstances { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Attachment> Attachments { get; set; }
        public DbSet<AttachmentType> AttachmentTypes { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<CareEvent> CareEvents { get; set; }
        public DbSet<CohortGroup> CohortGroups { get; set; }

        public DbSet<Config> Configs { get; set; }
        public DbSet<SystemLog> SystemLogs { get; set; }

        // Database keyless entities
        public DbSet<AdverseEventList> AdverseEventLists { get; set; }
        public DbSet<AdverseEventAnnualList> AdverseEventAnnualLists { get; set; }
        public DbSet<AdverseEventQuarterlyList> AdverseEventQuarterlyLists { get; set; }
        public DbSet<AppointmentList> AppointmentLists { get; set; }
        public DbSet<CausalityNotSetList> CausalityNotSetLists { get; set; }
        public DbSet<ContingencyAnalysisItem> ContingencyAnalysisItems { get; set; }
        public DbSet<ContingencyAnalysisList> ContingencyAnalysisLists { get; set; }
        public DbSet<ContingencyAnalysisPatient> ContingencyAnalysisPatients { get; set; }
        public DbSet<DrugList> DrugLists { get; set; }
        public DbSet<EncounterList> EncounterLists { get; set; }
        public DbSet<MetaPatientList> MetaPatientLists { get; set; }
        public DbSet<OutstandingVisitList> OutstandingVisitLists { get; set; }
        public DbSet<PatientList> PatientLists { get; set; }
        public DbSet<PatientOnStudyList> PatientOnStudyLists { get; set; }

        public PVIMSDbContext(DbContextOptions<PVIMSDbContext> options) : base(options) { }

        public PVIMSDbContext(DbContextOptions<PVIMSDbContext> options,
            IHttpContextAccessor httpContextAccessor) : base(options) 
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new ActivityEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new ActivityExecutionStatusEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new ActivityExecutionStatusEventEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new ActivityInstanceEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new AdverseEventAnnualListViewTypeConfiguration());
            modelBuilder.ApplyConfiguration(new AppointmentEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new AttachmentEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new AttachmentTypeEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new AuditLogEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new CareEventEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new CohortGroupEntityTypeConfiguration());
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
