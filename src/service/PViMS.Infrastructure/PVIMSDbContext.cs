﻿using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using PVIMS.Core.Entities.Accounts;
using PVIMS.Core.Entities;
using PVIMS.Core.Entities.Keyless;
using PVIMS.Core.SeedWork;
using PVIMS.Infrastructure.EntityConfigurations;

namespace PVIMS.Infrastructure
{
    public class PVIMSDbContext : DbContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        // Database entities
        public virtual DbSet<Activity> Activities { get; set; }
        public virtual DbSet<ActivityExecutionStatus> ActivityExecutionStatuses { get; set; }
        public virtual DbSet<ActivityExecutionStatusEvent> ActivityExecutionStatusEvents { get; set; }
        public virtual DbSet<ActivityInstance> ActivityInstances { get; set; }
        public virtual DbSet<Appointment> Appointments { get; set; }
        public virtual DbSet<Attachment> Attachments { get; set; }
        public virtual DbSet<AttachmentType> AttachmentTypes { get; set; }
        public virtual DbSet<AuditLog> AuditLogs { get; set; }
        public virtual DbSet<CareEvent> CareEvents { get; set; }
        public virtual DbSet<CohortGroup> CohortGroups { get; set; }
        public virtual DbSet<CohortGroupEnrolment> CohortGroupEnrolments { get; set; }
        public virtual DbSet<Concept> Concepts { get; set; }
        public virtual DbSet<ConceptIngredient> ConceptIngredients { get; set; }
        public virtual DbSet<Condition> Conditions { get; set; }
        public virtual DbSet<ConditionLabTest> ConditionLabTests { get; set; }
        public virtual DbSet<ConditionMedDra> ConditionMedDras { get; set; }
        public virtual DbSet<ConditionMedication> ConditionMedications { get; set; }
        public virtual DbSet<Config> Configs { get; set; }
        public virtual DbSet<ContextType> ContextTypes { get; set; }
        public virtual DbSet<CustomAttributeConfiguration> CustomAttributeConfigurations { get; set; }
        public virtual DbSet<Dataset> Datasets { get; set; }
        public virtual DbSet<DatasetCategory> DatasetCategories { get; set; }
        public virtual DbSet<DatasetCategoryCondition> DatasetCategoryConditions { get; set; }
        public virtual DbSet<DatasetCategoryElement> DatasetCategoryElements { get; set; }
        public virtual DbSet<DatasetCategoryElementCondition> DatasetCategoryElementConditions { get; set; }
        public virtual DbSet<DatasetElement> DatasetElements { get; set; }
        public virtual DbSet<DatasetElementSub> DatasetElementSubs { get; set; }
        public virtual DbSet<DatasetElementType> DatasetElementTypes { get; set; }
        public virtual DbSet<DatasetInstance> DatasetInstances { get; set; }
        public virtual DbSet<DatasetInstanceSubValue> DatasetInstanceSubValues { get; set; }
        public virtual DbSet<DatasetInstanceValue> DatasetInstanceValues { get; set; }
        public virtual DbSet<DatasetMapping> DatasetMappings { get; set; }
        public virtual DbSet<DatasetMappingSub> DatasetMappingSubs { get; set; }
        public virtual DbSet<DatasetMappingValue> DatasetMappingValues { get; set; }
        public virtual DbSet<DatasetRule> DatasetRules { get; set; }
        public virtual DbSet<DatasetXml> DatasetXmls { get; set; }
        public virtual DbSet<DatasetXmlAttribute> DatasetXmlAttributes { get; set; }
        public virtual DbSet<DatasetXmlNode> DatasetXmlNodes { get; set; }
        public virtual DbSet<Encounter> Encounters { get; set; }
        public virtual DbSet<EncounterType> EncounterTypes { get; set; }
        public virtual DbSet<EncounterTypeWorkPlan> EncounterTypeWorkPlans { get; set; }
        public virtual DbSet<Facility> Facilities { get; set; }
        public virtual DbSet<FacilityType> FacilityTypes { get; set; }
        public virtual DbSet<Field> Fields { get; set; }
        public virtual DbSet<FieldType> FieldTypes { get; set; }
        public virtual DbSet<FieldValue> FieldValues { get; set; }
        public virtual DbSet<Holiday> Holidays { get; set; }
        public virtual DbSet<LabResult> LabResults { get; set; }
        public virtual DbSet<LabTest> LabTests { get; set; }
        public virtual DbSet<LabTestUnit> LabTestUnits { get; set; }
        public virtual DbSet<Language> Languages { get; set; }
        public virtual DbSet<MedDRAGrading> MedDragradings { get; set; }
        public virtual DbSet<MedDRAScale> MedDrascales { get; set; }
        public virtual DbSet<MedicationForm> MedicationForms { get; set; }
        public virtual DbSet<MetaColumn> MetaColumns { get; set; }
        public virtual DbSet<MetaColumnType> MetaColumnTypes { get; set; }
        public virtual DbSet<MetaDependency> MetaDependencies { get; set; }
        public virtual DbSet<MetaForm> MetaForms { get; set; }
        public virtual DbSet<MetaPage> MetaPages { get; set; }
        public virtual DbSet<MetaReport> MetaReports { get; set; }
        public virtual DbSet<MetaTable> MetaTables { get; set; }
        public virtual DbSet<MetaTableType> MetaTableTypes { get; set; }
        public virtual DbSet<MetaWidget> MetaWidgets { get; set; }
        public virtual DbSet<MetaWidgetType> MetaWidgetTypes { get; set; }
        public virtual DbSet<OrgUnit> OrgUnits { get; set; }
        public virtual DbSet<OrgUnitType> OrgUnitTypes { get; set; }
        public virtual DbSet<Outcome> Outcomes { get; set; }
        public virtual DbSet<Patient> Patients { get; set; }
        public virtual DbSet<PatientClinicalEvent> PatientClinicalEvents { get; set; }
        public virtual DbSet<PatientCondition> PatientConditions { get; set; }
        public virtual DbSet<PatientFacility> PatientFacilities { get; set; }
        public virtual DbSet<PatientLabTest> PatientLabTests { get; set; }
        public virtual DbSet<PatientLanguage> PatientLanguages { get; set; }
        public virtual DbSet<PatientMedication> PatientMedications { get; set; }
        public virtual DbSet<PatientStatus> PatientStatuses { get; set; }
        public virtual DbSet<PatientStatusHistory> PatientStatusHistories { get; set; }
        public virtual DbSet<PostDeployment> PostDeployments { get; set; }
        public virtual DbSet<Pregnancy> Pregnancies { get; set; }
        public virtual DbSet<Priority> Priorities { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<RefreshToken> RefreshTokens { get; set; }
        public virtual DbSet<ReportInstance> ReportInstances { get; set; }
        public virtual DbSet<ReportInstanceMedication> ReportInstanceMedications { get; set; }
        public virtual DbSet<RiskFactor> RiskFactors { get; set; }
        public virtual DbSet<RiskFactorOption> RiskFactorOptions { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<SelectionDataItem> SelectionDataItems { get; set; }
        public virtual DbSet<SiteContactDetail> SiteContactDetails { get; set; }
        public virtual DbSet<SystemLog> SystemLogs { get; set; }
        public virtual DbSet<TerminologyIcd10> TerminologyIcd10s { get; set; }
        public virtual DbSet<TerminologyMedDra> TerminologyMedDras { get; set; }
        public virtual DbSet<TreatmentOutcome> TreatmentOutcomes { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserFacility> UserFacilities { get; set; }
        public virtual DbSet<UserRole> UserRoles { get; set; }
        public virtual DbSet<WorkFlow> WorkFlows { get; set; }
        public virtual DbSet<WorkPlan> WorkPlans { get; set; }
        public virtual DbSet<WorkPlanCareEvent> WorkPlanCareEvents { get; set; }
        public virtual DbSet<WorkPlanCareEventDatasetCategory> WorkPlanCareEventDatasetCategories { get; set; }

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
            modelBuilder.ApplyConfiguration(new CohortGroupEnrolmentEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new ConceptEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new ConceptIngredientEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new ConditionEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new ConditionLabTestEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new ConditionMedDraEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new ConditionMedicationEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new ConfigEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new CustomAttributeConfigurationEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new DatasetEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new DatasetCategoryEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new DatasetCategoryConditionEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new DatasetCategoryElementEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new DatasetCategoryElementConditionEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new DatasetElementEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new DatasetElementSubEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new DatasetElementTypeEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new DatasetInstanceEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new DatasetInstanceSubValueEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new DatasetInstanceValueEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new DatasetMappingEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new DatasetMappingSubEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new DatasetMappingValueEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new DatasetRuleEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new DatasetXmlEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new DatasetXmlAttributeEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new DatasetXmlNodeEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new EncounterEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new EncounterTypeEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new EncounterTypeWorkPlanEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new FacilityEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new FacilityTypeEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new FieldEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new FieldTypeEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new FieldValueEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new HolidayEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new LabResultEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new LabTestEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new LabTestUnitEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new LanguageEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new MedDRAGradingEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new MedDRAScaleEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new MedicationFormEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new MetaColumnEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new MetaColumnTypeEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new MetaDependencyEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new MetaFormEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new MetaPageEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new MetaReportEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new MetaTableEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new MetaTableTypeEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new MetaWidgetEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new MetaWidgetTypeEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new OrgUnitEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new OutcomeEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new PatientEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new PatientClinicalEventEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new PatientConditionEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new PatientFacilityEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new PatientLabTestEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new PatientLanguageEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new PatientMedicationEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new PatientStatusEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new PatientStatusHistoryEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new PostDeploymentEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new PregnancyEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new PriorityEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new ProductEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new RefreshTokenEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new ReportInstanceEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new ReportInstanceMedicationEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new RiskFactorEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new RiskFactorOptionEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new RoleEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new SelectionDataItemEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new SiteContactDetailEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new SystemLogEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new TerminologyIcd10EntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new TerminologyMedDraEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new TreatmentOutcomeEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new UserEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new UserFacilityEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new UserRoleEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new WorkFlowEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new WorkPlanEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new WorkPlanCareEventEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new WorkPlanCareEventDatasetCategoryEntityTypeConfiguration());
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