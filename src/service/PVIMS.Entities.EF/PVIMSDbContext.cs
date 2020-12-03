using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.Migrations;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;
using System.Transactions;

using VPS.CustomAttributes;
using PVIMS.Core.Entities;
using VPS.EF;
using VPS.Common.Domain;
using PVIMS.Core;

using SelectionDataItem = PVIMS.Core.Entities.SelectionDataItem;
using CustomAttributeConfiguration = PVIMS.Core.Entities.CustomAttributeConfiguration;
using PVIMS.Core.ValueTypes;
using PVIMS.Core.Entities.Accounts;

namespace PVIMS.Entities.EF
{
    public class PVIMSDbContext : NonPluralDbContextBase
    {
        private readonly UserContext _userContext;

        public PVIMSDbContext()
            : base("PVIMS")
        {
#if DEBUG
            this.Database.Log = message => Debug.WriteLine(message);
#endif
        }

        public PVIMSDbContext(UserContext userContext)
            : base("PVIMS")
        {
            _userContext = userContext ?? throw new ArgumentNullException(nameof(userContext));
#if DEBUG
            this.Database.Log = message => Debug.WriteLine(message);
#endif
        }

        public PVIMSDbContext(string connectionString, UserContext userContext)
            : base(connectionString)
        {
            _userContext = userContext ?? throw new ArgumentNullException(nameof(userContext));

            Database.CommandTimeout = 90;
#if DEBUG
            //Database.Log = Console.WriteLine;
            //DbInterception.Add(new CommonLoggingCommandInterceptor());
#endif
        }

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
        public virtual DbSet<ConditionMedication> ConditionMedications { get; set; }
        public virtual DbSet<ConditionMedDra> ConditionMedDras { get; set; }
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
        public virtual DbSet<DatasetInstanceValue> DatasetInstanceValues { get; set; }
        public virtual DbSet<DatasetInstanceSubValue> DatasetInstanceSubValues { get; set; }
        public virtual DbSet<DatasetMapping> DatasetMappings { get; set; }
        public virtual DbSet<DatasetMappingValue> DatasetMappingValues { get; set; }
        public virtual DbSet<DatasetRule> DatasetRules { get; set; }
        public virtual DbSet<DatasetXml> DatasetXmls { get; set; }
        public virtual DbSet<DatasetXmlNode> DatasetXmlNodes { get; set; }
        public virtual DbSet<DatasetXmlAttribute> DatasetXmlAttributes { get; set; }
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
        public virtual DbSet<MedicationForm> MedicationForms { get; set; }
        public virtual DbSet<MedDRAGrading> MedDRAGradings { get; set; }
        public virtual DbSet<MedDRAScale> MedDRAScales { get; set; }
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
        public virtual DbSet<PatientStatus> PatientStatus { get; set; }
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
        public virtual DbSet<TerminologyIcd10> TerminologyIcd10 { get; set; }
        public virtual DbSet<TerminologyMedDra> TerminologyMedDras { get; set; }
        public virtual DbSet<TreatmentOutcome> TreatmentOutcomes { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserFacility> UserFacilities { get; set; }
        public virtual DbSet<UserRole> UserRoles { get; set; }
        public virtual DbSet<WorkFlow> WorkFlows { get; set; }
        public virtual DbSet<WorkPlan> WorkPlans { get; set; }
        public virtual DbSet<WorkPlanCareEvent> WorkPlanCareEvents { get; set; }
        public virtual DbSet<WorkPlanCareEventDatasetCategory> WorkPlanCareEventDatasetCategories { get; set; }

        public override int SaveChanges()
        {
            foreach (var changedAuditedEntity in ChangeTracker.Entries().Where(IsAuditedEntityThatHasChanged).Select(e => (AuditedEntity<int, User>)e.Entity))
            {
                // Need to retrieve the current user from the current dbcontext because UserContext.CurrentUser is populated, in the HttpContextUserContext object, from 
                // a different dbcontext. A duplicate User record is inserted in the DB when UserContext.CurrentUser is passed in to the AuditStamp operation, resulting in a 
                // duplicate user record. Not sure if the retrieval of a user from different dbcontexts is the root cause of the duplication but, adding the 
                // user retrieval operation below solves the issue of duplicate users being created.
                try
                {
                    User currentUser = null;
                    if (Users != null && _userContext != null)
                    {
                        currentUser = _userContext != null ? Users.SingleOrDefault(u => u.UserName == _userContext.UserName) : null;
                    }
                    changedAuditedEntity.AuditStamp(currentUser);
                }
                catch (Exception)
                {
                    changedAuditedEntity.AuditStamp(null);
                }
            }

#if DEBUG
            try
            {
#endif
                return base.SaveChanges();
#if DEBUG
            }
            catch (DbEntityValidationException exception)
            {
                var err = string.Empty;
                foreach (var eve in exception.EntityValidationErrors)
                {
                    foreach (var ve in eve.ValidationErrors)
                    {
                        err += String.Format("- Property: \"{0}\", Error: \"{1}\"", ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw new Exception(err);
            }
            catch (Exception exception)
            {
                throw; // provided for debugging reasons.
            }
#endif
        }

        private bool IsAuditedEntityThatHasChanged(DbEntityEntry entry)
        {
            if (entry.State == EntityState.Unchanged) return false;

            if (entry.State == EntityState.Deleted) return false;

            return typeof(AuditedEntity<int, User>).IsAssignableFrom(entry.Entity.GetType());
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Appointment>()
                .HasOptional<User>(s => s.AuditUser)
                .WithMany(s => s.Appointments)
                .HasForeignKey(s => s.AuditUser_Id);

            modelBuilder.Entity<Attachment>()
                .HasOptional<User>(s => s.AuditUser)
                .WithMany(s => s.Attachments)
                .HasForeignKey(s => s.AuditUser_Id);

            modelBuilder.Entity<Encounter>()
                .HasOptional<User>(s => s.AuditUser)
                .WithMany(s => s.Encounters)
                .HasForeignKey(s => s.AuditUser_Id);

            modelBuilder.Entity<Patient>()
                .HasOptional<User>(s => s.AuditUser)
                .WithMany(s => s.Patients)
                .HasForeignKey(s => s.AuditUser_Id);

            modelBuilder.Entity<PatientStatusHistory>()
                .HasOptional<User>(s => s.AuditUser)
                .WithMany(s => s.PatientStatusHistories)
                .HasForeignKey(s => s.AuditUser_Id);

            modelBuilder.Entity<PostDeployment>()
                .Property(f => f.RunDate)
                .HasColumnType("datetime2")
                .HasPrecision(0);

            modelBuilder.Entity<PostDeployment>()
                .Property(f => f.ScriptFileName)
                .HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute { IsUnique = true }));
        }

        public class CreateInitializer : DropCreateDatabaseAlways<PVIMSDbContext>
        {
            protected override void Seed(PVIMSDbContext context)
            {
                context.Seed(context);

                base.Seed(context);
            }
        }


        public void Seed(PVIMSDbContext context)
        {
            if (context.Roles.Any()) return;

            using (var transaction = new TransactionScope())
            {
                CreateRoles(context);
                CreateUser(context);

                CreateStatus(context);
                CreateAttachmentTypes(context);
                CreateOutcomes(context);
                CreateTreatmentOutcomes(context);
                CreatePriorities(context);
                CreateFacilityTypes(context);
                CreateConfigValues(context);
                CreateContactTypes(context);
                CreateContextTypes(context);
                CreatePostDeploymentScripts(context);

                CreateCareEvents(context);
                CreateDatasetElementTypes(context);
                CreateFieldTypes(context);
                CreateMetaTypes(context);

                CreateLabResults(context);
                CreateLabTestUnits(context);
                
                CreateCustomAttributes(context);
                CreateSelectDataItem(context);

                transaction.Complete();
            }
        }

        private static void CreateUser(PVIMSDbContext context)
        {
            // Password: P@55w0rd1
            var adminUser = new User
            {
                UserName = "Admin",
                FirstName = "Admin",
                LastName = "User",
                Active = true,
                Email = "admin@mail.com",
                PasswordHash = "AGW4fU8+b1lKknBmk1U78xlZwJvxihua6thtRmUJRFZhgFepPm9FfgnXiUMe2SkVEw=="
            };
            context.Users.AddOrUpdate(u => u.UserName, adminUser);

            context.UserRoles.Add(new UserRole
            {
                User = adminUser,
                Role = context.Roles.SingleOrDefault(r => r.Key == "Admin")
            });
            context.UserRoles.Add(new UserRole
            {
                User = adminUser,
                Role = context.Roles.SingleOrDefault(r => r.Key == "DataCap")
            });
            context.UserRoles.Add(new UserRole
            {
                User = adminUser,
                Role = context.Roles.SingleOrDefault(r => r.Key == "Analyst")
            });
            context.UserRoles.Add(new UserRole
            {
                User = adminUser,
                Role = context.Roles.SingleOrDefault(r => r.Key == "Reporter")
            });
            context.UserRoles.Add(new UserRole
            {
                User = adminUser,
                Role = context.Roles.SingleOrDefault(r => r.Key == "Publisher")
            });
            context.UserRoles.Add(new UserRole
            {
                User = adminUser,
                Role = context.Roles.SingleOrDefault(r => r.Key == "ReporterAdmin")
            });
            context.UserRoles.Add(new UserRole
            {
                User = adminUser,
                Role = context.Roles.SingleOrDefault(r => r.Key == "PublisherAdmin")
            });
            context.SaveChanges();
        }

        private static void CreateStatus(PVIMSDbContext context)
        {
            var statuses = new[]
            {
                new PatientStatus {Description ="Active"},
                new PatientStatus {Description ="Suspended"},
                new PatientStatus {Description ="Stopped ART"},
                new PatientStatus {Description ="Investigation"},
                new PatientStatus {Description ="LTF"},
                new PatientStatus {Description ="Stopped PMTCT"},
                new PatientStatus {Description ="Transferred Out"},
                new PatientStatus {Description ="Died"},
            };

            context.PatientStatus.AddOrUpdate(s => new { s.Description }, statuses);
            context.SaveChanges();
        }

        private static void CreateRoles(PVIMSDbContext context)
        {
            var roles = new[]
			{
				new Role { Name = "Administrator", Key = "Admin" },
				new Role { Name = "Registration Clerk", Key = "RegClerk" },
				new Role { Name = "Data Capturer", Key = "DataCap" },
                new Role { Name = "Clinician", Key = "Clinician" },
                new Role { Name = "Analytics", Key = "Analyst" },
                new Role { Name = "Reporter", Key = "Reporter" },
                new Role { Name = "Publisher", Key = "Publisher" },
                new Role { Name = "Reporter Administrator", Key = "ReporterAdmin" },
                new Role { Name = "Publisher Administrator", Key = "PublisherAdmin" }
			};

            context.Roles.AddOrUpdate(r => new { r.Name, r.Key }, roles);
            context.SaveChanges();
        }

        private static void CreateAttachmentTypes(PVIMSDbContext context)
        {
            var attachmentTypes = new[]
			{
				new AttachmentType {Description = "MS Word 2003-2007 Document", Key = "doc"},
				new AttachmentType {Description = "MS Excel 2003-2007 Document", Key = "xls"},
				new AttachmentType {Description = "MS Word Document", Key = "docx"},
				new AttachmentType {Description = "MS Excel Document", Key = "xlsx"},
                new AttachmentType {Description = "Portable Document Format", Key = "pdf"},
				new AttachmentType {Description = "Image | JPEG", Key = "jpg"},
				new AttachmentType {Description = "Image | JPEG", Key = "jpeg"},
				new AttachmentType {Description = "Image | PNG", Key = "png"},
				new AttachmentType {Description = "Image | BMP", Key = "bmp"},
                new AttachmentType {Description = "XML Document", Key = "xml"}
            };

            context.AttachmentTypes.AddOrUpdate(at => new { at.Description, at.Key }, attachmentTypes);
            context.SaveChanges();
        }

        private static void CreatePriorities(PVIMSDbContext context)
        {
            var priorities = new[]
			{
                new Priority { Description = "Not Set" },
				new Priority { Description = "Urgent" },
				new Priority { Description = "High" },
				new Priority { Description = "Medium" },
                new Priority { Description = "Low" }
			};

            context.Priorities.AddOrUpdate(p => new { p.Description }, priorities);
            context.SaveChanges();
        }

        private static void CreateCareEvents(PVIMSDbContext context)
        {
            var careEvents = new[]
			{
				new CareEvent {Description = "Capture Vitals"},
				new CareEvent {Description = "Counsel and Pill Count"},
				new CareEvent {Description = "Doctor Assessment"},
				new CareEvent {Description = "Nurse Assessment"},
				new CareEvent {Description = "Drug Dispensement"}
			};

            context.CareEvents.AddOrUpdate(ce => ce.Description, careEvents);
            context.SaveChanges();
        }

        private static void CreateDatasetElementTypes(PVIMSDbContext context)
        {
            var elementTypes = new[]
			{
				new DatasetElementType {Description = "Generic"}
			};

            context.DatasetElementTypes.AddOrUpdate(det => det.Description, elementTypes);
            context.SaveChanges();
        }

        private static void CreateLabResults(PVIMSDbContext context)
        {
            var labResults = new[]
			{
                new LabResult { Description = "Positive" },
                new LabResult { Description = "Negative" },
                new LabResult { Description = "Borderline" },
                new LabResult { Description = "Inconclusive" },
                new LabResult { Description = "Normal" },
                new LabResult { Description = "Abnormal" },
                new LabResult { Description = "Seronegative" },
                new LabResult { Description = "Seropositive" },
                new LabResult { Description = "Improved" },
                new LabResult { Description = "Stable" },
                new LabResult { Description = "Progressed" }
			};

            context.LabResults.AddOrUpdate(lt => lt.Description, labResults);
            context.SaveChanges();
        }

        private static void CreateLabTestUnits(PVIMSDbContext context)
        {
            var labTestUnits = new[]
			{
                new LabTestUnit { Description = "N/A" },
                new LabTestUnit { Description = "%" },
                new LabTestUnit { Description = "% hearing loss left ear" },
                new LabTestUnit { Description = "% hearing loss right ear" },
                new LabTestUnit { Description = "µg/dL" },
                new LabTestUnit { Description = "µg/L" },
                new LabTestUnit { Description = "beats per minute" },
                new LabTestUnit { Description = "breaths per minute" },
                new LabTestUnit { Description = "cavities" },
                new LabTestUnit { Description = "cells/mm 3" },
                new LabTestUnit { Description = "g/dL" },
                new LabTestUnit { Description = "g/L" },
                new LabTestUnit { Description = "IU/L" },
                new LabTestUnit { Description = "kg/m 2" },
                new LabTestUnit { Description = "mEq/L" },
                new LabTestUnit { Description = "mg/24 hr" },
                new LabTestUnit { Description = "mg/dL" },
                new LabTestUnit { Description = "min" },
                new LabTestUnit { Description = "mL/min" },
                new LabTestUnit { Description = "mm Hg" },
                new LabTestUnit { Description = "mm/h" },
                new LabTestUnit { Description = "mmol/kg" },
                new LabTestUnit { Description = "mmol/L" },
                new LabTestUnit { Description = "mOsm/kg" },
                new LabTestUnit { Description = "ms" },
                new LabTestUnit { Description = "ng/dL" },
                new LabTestUnit { Description = "ng/L" },
                new LabTestUnit { Description = "ng/mL" },
                new LabTestUnit { Description = "ng/mL/hr" },
                new LabTestUnit { Description = "nmol/L" },
                new LabTestUnit { Description = "pg/mL" },
                new LabTestUnit { Description = "pH" },
                new LabTestUnit { Description = "pmol/L" },
                new LabTestUnit { Description = "sec" },
                new LabTestUnit { Description = "U/L" },
                new LabTestUnit { Description = "X 10 3 /mm 3" },
                new LabTestUnit { Description = "X 10 6 /mm 3" },
                new LabTestUnit { Description = "X 10 9 /L" },
                new LabTestUnit { Description = "μg/dL" },
                new LabTestUnit { Description = "μg/L" },
                new LabTestUnit { Description = "μmol/L" },
                new LabTestUnit { Description = "μU/L" },
                new LabTestUnit { Description = "μU/mL" }
			};

            context.LabTestUnits.AddOrUpdate(lt => lt.Description, labTestUnits);
            context.SaveChanges();
        }

        private static void CreateOutcomes(PVIMSDbContext context)
        {
            var outcomes = new[]
			{
				new Outcome { Description = "Recovered/Resolved" },
				new Outcome { Description = "Recovered/Resolved With Sequelae" },
				new Outcome { Description = "Recovering/Resolving" },
                new Outcome { Description = "Not Recovered/Not Resolved" },
                new Outcome { Description = "Fatal" },
                new Outcome { Description = "Unknown" }
			};

            context.Outcomes.AddOrUpdate(p => p.Description, outcomes);
            context.SaveChanges();
        }

        private static void CreateTreatmentOutcomes(PVIMSDbContext context)
        {
            var treatmentOutcomes = new[]
			{
				new TreatmentOutcome { Description = "Cured" },
				new TreatmentOutcome { Description = "Treatment Completed" },
				new TreatmentOutcome { Description = "Treatment Failed" },
                new TreatmentOutcome { Description = "Died" },
                new TreatmentOutcome { Description = "Lost to Follow-up" },
                new TreatmentOutcome { Description = "Not Evaluated" }
			};

            context.TreatmentOutcomes.AddOrUpdate(p => p.Description, treatmentOutcomes);
            context.SaveChanges();
        }

        private static void CreateCustomAttributes(PVIMSDbContext context)
        {
            var customAttributeConfigs = new[]
			{
				new CustomAttributeConfiguration
				{
					ExtendableTypeName = "PatientClinicalEvent",
					CustomAttributeType = CustomAttributeType.String,
					Category = "Custom",
					AttributeKey = "SAE Number",
                    StringMaxLength = 50
				},
				new CustomAttributeConfiguration
				{
					ExtendableTypeName = "PatientClinicalEvent",
					CustomAttributeType = CustomAttributeType.Selection,
					Category = "Custom",
					AttributeKey = "Outcome",
                    AttributeDetail = "For fatal outcomes, please ensure all conditions are updated to reflect the relevant condition outcome"
				},
				new CustomAttributeConfiguration
				{
					ExtendableTypeName = "PatientClinicalEvent",
					CustomAttributeType = CustomAttributeType.Selection,
					Category = "Custom",
					AttributeKey = "Intensity (Severity)"
				},
				new CustomAttributeConfiguration
				{
					ExtendableTypeName = "PatientClinicalEvent",
					CustomAttributeType = CustomAttributeType.Selection,
					Category = "Custom",
					AttributeKey = "Severity Grading Scale"
				},
				new CustomAttributeConfiguration
				{
					ExtendableTypeName = "PatientClinicalEvent",
					CustomAttributeType = CustomAttributeType.Selection,
					Category = "Custom",
					AttributeKey = "Severity Grade"
				},
				new CustomAttributeConfiguration
				{
					ExtendableTypeName = "PatientClinicalEvent",
					CustomAttributeType = CustomAttributeType.Selection,
					Category = "Custom",
					AttributeKey = "Is the adverse event serious?"
				},
				new CustomAttributeConfiguration
				{
					ExtendableTypeName = "PatientClinicalEvent",
					CustomAttributeType = CustomAttributeType.Selection,
					Category = "Custom",
					AttributeKey = "Seriousness"
				},
				new CustomAttributeConfiguration
				{
					ExtendableTypeName = "PatientClinicalEvent",
					CustomAttributeType = CustomAttributeType.DateTime,
					Category = "Custom",
					AttributeKey = "Admission Date",
                    PastDateOnly = true
				},
				new CustomAttributeConfiguration
				{
					ExtendableTypeName = "PatientClinicalEvent",
					CustomAttributeType = CustomAttributeType.DateTime,
					Category = "Custom",
					AttributeKey = "Discharge Date",
                    PastDateOnly = true
				},
				new CustomAttributeConfiguration
				{
					ExtendableTypeName = "PatientClinicalEvent",
					CustomAttributeType = CustomAttributeType.DateTime,
					Category = "Custom",
					AttributeKey = "Date of Death",
                    PastDateOnly = true
				},
				new CustomAttributeConfiguration
				{
					ExtendableTypeName = "PatientClinicalEvent",
					CustomAttributeType = CustomAttributeType.Selection,
					Category = "Custom",
					AttributeKey = "Autopsy Done"
				},
				new CustomAttributeConfiguration
				{
					ExtendableTypeName = "PatientClinicalEvent",
					CustomAttributeType = CustomAttributeType.Selection,
					Category = "Custom",
					AttributeKey = "Was the AE attributed to one or more drugs?"
				},
				new CustomAttributeConfiguration
				{
					ExtendableTypeName = "PatientClinicalEvent",
					CustomAttributeType = CustomAttributeType.Selection,
					Category = "Custom",
					AttributeKey = "Was the event reported to national PV?"
				},
				new CustomAttributeConfiguration
				{
					ExtendableTypeName = "PatientClinicalEvent",
					CustomAttributeType = CustomAttributeType.String,
					Category = "Custom",
					AttributeKey = "Reported By",
                    StringMaxLength = 50
				},
				new CustomAttributeConfiguration
				{
					ExtendableTypeName = "PatientClinicalEvent",
					CustomAttributeType = CustomAttributeType.DateTime,
					Category = "Custom",
					AttributeKey = "Date of Report",
                    PastDateOnly = true
				},
				new CustomAttributeConfiguration
				{
					ExtendableTypeName = "PatientClinicalEvent",
					CustomAttributeType = CustomAttributeType.String,
					Category = "Custom",
					AttributeKey = "Comments",
                    StringMaxLength = 100
				},
				new CustomAttributeConfiguration
				{
					ExtendableTypeName = "PatientCondition",
					CustomAttributeType = CustomAttributeType.Selection,
					Category = "Custom",
					AttributeKey = "Condition Ongoing"
				},
				new CustomAttributeConfiguration
				{
					ExtendableTypeName = "PatientMedication",
					CustomAttributeType = CustomAttributeType.Selection,
					Category = "Custom",
					AttributeKey = "Route"
				},
				new CustomAttributeConfiguration
				{
					ExtendableTypeName = "PatientMedication",
					CustomAttributeType = CustomAttributeType.Selection,
					Category = "Custom",
					AttributeKey = "Frequency in days per week"
				},
				new CustomAttributeConfiguration
				{
					ExtendableTypeName = "PatientMedication",
					CustomAttributeType = CustomAttributeType.Selection,
					Category = "Custom",
					AttributeKey = "Still On Medication"
				},
				new CustomAttributeConfiguration
				{
					ExtendableTypeName = "PatientMedication",
					CustomAttributeType = CustomAttributeType.String,
					Category = "Custom",
					AttributeKey = "Indication",
                    StringMaxLength = 50
				},
				new CustomAttributeConfiguration
				{
					ExtendableTypeName = "PatientMedication",
					CustomAttributeType = CustomAttributeType.Selection,
					Category = "Custom",
					AttributeKey = "Type of Indication"
				},
				new CustomAttributeConfiguration
				{
					ExtendableTypeName = "PatientMedication",
					CustomAttributeType = CustomAttributeType.Selection,
					Category = "Custom",
					AttributeKey = "Reason For Stopping"
				},
				new CustomAttributeConfiguration
				{
					ExtendableTypeName = "PatientMedication",
					CustomAttributeType = CustomAttributeType.Selection,
					Category = "Custom",
					AttributeKey = "Clinician action taken with regard to medicine if related to AE"
				},
				new CustomAttributeConfiguration
				{
					ExtendableTypeName = "PatientMedication",
					CustomAttributeType = CustomAttributeType.String,
					Category = "Custom",
					AttributeKey = "Batch Number",
                    StringMaxLength = 50
				},
				new CustomAttributeConfiguration
				{
					ExtendableTypeName = "PatientMedication",
					CustomAttributeType = CustomAttributeType.Selection,
					Category = "Custom",
					AttributeKey = "Effect OF Dechallenge (D) & Rechallenge (R)"
				},
				new CustomAttributeConfiguration
				{
					ExtendableTypeName = "PatientMedication",
					CustomAttributeType = CustomAttributeType.String,
					Category = "Custom",
					AttributeKey = "Comments",
                    StringMaxLength = 100
				},
				new CustomAttributeConfiguration
				{
					ExtendableTypeName = "Patient",
					CustomAttributeType = CustomAttributeType.String,
					Category = "Custom",
					AttributeKey = "Medical Record Number",
                    IsRequired = true,
                    StringMaxLength = 50,
                    IsSearchable = true
				},
				new CustomAttributeConfiguration
				{
					ExtendableTypeName = "Patient",
					CustomAttributeType = CustomAttributeType.String,
					Category = "Custom",
					AttributeKey = "Patient Identity Number",
                    IsRequired = true,
                    StringMaxLength = 11
				},
				new CustomAttributeConfiguration
				{
					ExtendableTypeName = "Patient",
					CustomAttributeType = CustomAttributeType.Selection,
					Category = "Custom",
					AttributeKey = "Identity Type",
                    IsRequired = true
				},
				new CustomAttributeConfiguration
				{
					ExtendableTypeName = "Patient",
					CustomAttributeType = CustomAttributeType.Selection,
					Category = "Custom",
					AttributeKey = "Gender",
                    IsRequired = true
				},
				new CustomAttributeConfiguration
				{
					ExtendableTypeName = "Patient",
					CustomAttributeType = CustomAttributeType.Selection,
					Category = "Custom",
					AttributeKey = "Marital Status"
				},
				new CustomAttributeConfiguration
				{
					ExtendableTypeName = "Patient",
					CustomAttributeType = CustomAttributeType.Selection,
					Category = "Custom",
					AttributeKey = "Employment Status"
				},
				new CustomAttributeConfiguration
				{
					ExtendableTypeName = "Patient",
					CustomAttributeType = CustomAttributeType.String,
					Category = "Custom",
					AttributeKey = "Occupation",
                    StringMaxLength = 50
				},
				new CustomAttributeConfiguration
				{
					ExtendableTypeName = "Patient",
					CustomAttributeType = CustomAttributeType.Selection,
					Category = "Custom",
					AttributeKey = "Language"
				},
				new CustomAttributeConfiguration
				{
					ExtendableTypeName = "Patient",
					CustomAttributeType = CustomAttributeType.String,
					Category = "Custom",
					AttributeKey = "Address",
                    StringMaxLength = 100
				},
				new CustomAttributeConfiguration
				{
					ExtendableTypeName = "Patient",
					CustomAttributeType = CustomAttributeType.String,
					Category = "Custom",
					AttributeKey = "Address Line 2",
                    StringMaxLength = 100
				},
				new CustomAttributeConfiguration
				{
					ExtendableTypeName = "Patient",
					CustomAttributeType = CustomAttributeType.String,
					Category = "Custom",
					AttributeKey = "City",
                    StringMaxLength = 50
				},
				new CustomAttributeConfiguration
				{
					ExtendableTypeName = "Patient",
					CustomAttributeType = CustomAttributeType.String,
					Category = "Custom",
					AttributeKey = "State",
                    StringMaxLength = 50
				},
				new CustomAttributeConfiguration
				{
					ExtendableTypeName = "Patient",
					CustomAttributeType = CustomAttributeType.String,
					Category = "Custom",
					AttributeKey = "Postal Code",
                    StringMaxLength = 10
				},
				new CustomAttributeConfiguration
				{
					ExtendableTypeName = "Patient",
					CustomAttributeType = CustomAttributeType.String,
					Category = "Custom",
					AttributeKey = "Patient Contact Number",
                    StringMaxLength = 15
				},
                new CustomAttributeConfiguration
                {
                    ExtendableTypeName = "PatientLabTest",
                    CustomAttributeType = CustomAttributeType.String,
                    Category = "Lab Test",
                    AttributeKey = "Comments",
                    StringMaxLength = 255
                }
            };

            context.CustomAttributeConfigurations.AddOrUpdate(c => new { c.AttributeKey, c.ExtendableTypeName }, customAttributeConfigs);
            context.SaveChanges();
        }

        private static void CreateSelectDataItem(PVIMSDbContext context)
        {
            var selectionDataItems = new[]
			{
				new SelectionDataItem {AttributeKey = "Outcome", Value = "", SelectionKey = "0"},
				new SelectionDataItem {AttributeKey = "Outcome", Value = "Resolved", SelectionKey = "1"},
				new SelectionDataItem {AttributeKey = "Outcome", Value = "Resolved with sequelae", SelectionKey = "2"},
				new SelectionDataItem {AttributeKey = "Outcome", Value = "Fatal", SelectionKey = "3"},
				new SelectionDataItem {AttributeKey = "Outcome", Value = "Resolving", SelectionKey = "4"},
                new SelectionDataItem {AttributeKey = "Outcome", Value = "Not resolved", SelectionKey = "5"},
				new SelectionDataItem {AttributeKey = "Outcome", Value = "Unknown", SelectionKey = "6"},
				new SelectionDataItem {AttributeKey = "Intensity (Severity)", Value = "", SelectionKey = "0"},
                new SelectionDataItem {AttributeKey = "Intensity (Severity)", Value = "Mild", SelectionKey = "1"},
                new SelectionDataItem {AttributeKey = "Intensity (Severity)", Value = "Moderate", SelectionKey = "2"},
                new SelectionDataItem {AttributeKey = "Intensity (Severity)", Value = "Severe", SelectionKey = "3"},
				new SelectionDataItem {AttributeKey = "Severity Grading Scale", Value = "", SelectionKey = "0"},
                new SelectionDataItem {AttributeKey = "Severity Grading Scale", Value = "WHO Scale", SelectionKey = "1"},
                new SelectionDataItem {AttributeKey = "Severity Grading Scale", Value = "Clinician’s judgement", SelectionKey = "2"},
                new SelectionDataItem {AttributeKey = "Severity Grading Scale", Value = "CTCAE grading system", SelectionKey = "3"},
                new SelectionDataItem {AttributeKey = "Severity Grading Scale", Value = "DAIDS AE Grading Table", SelectionKey = "4"},
                new SelectionDataItem {AttributeKey = "Severity Grading Scale", Value = "Other", SelectionKey = "5"},
				new SelectionDataItem {AttributeKey = "Severity Grade", Value = "", SelectionKey = "0"},
                new SelectionDataItem {AttributeKey = "Severity Grade", Value = "Grade 1", SelectionKey = "1"},
                new SelectionDataItem {AttributeKey = "Severity Grade", Value = "Grade 2", SelectionKey = "2"},
                new SelectionDataItem {AttributeKey = "Severity Grade", Value = "Grade 3", SelectionKey = "3"},
                new SelectionDataItem {AttributeKey = "Severity Grade", Value = "Grade 4", SelectionKey = "4"},
                new SelectionDataItem {AttributeKey = "Severity Grade", Value = "Grade 5", SelectionKey = "5"},
				new SelectionDataItem {AttributeKey = "Is the adverse event serious?", Value = "", SelectionKey = "0"},
                new SelectionDataItem {AttributeKey = "Is the adverse event serious?", Value = "Yes", SelectionKey = "1"},
                new SelectionDataItem {AttributeKey = "Is the adverse event serious?", Value = "No", SelectionKey = "2"},
                new SelectionDataItem {AttributeKey = "Is the adverse event serious?", Value = "Unknown", SelectionKey = "3"},
				new SelectionDataItem {AttributeKey = "Seriousness", Value = "", SelectionKey = "0"},
                new SelectionDataItem {AttributeKey = "Seriousness", Value = "A congenital anomaly or birth defect", SelectionKey = "1"},
                new SelectionDataItem {AttributeKey = "Seriousness", Value = "Persistent or significant disability or incapacity", SelectionKey = "2"},
                new SelectionDataItem {AttributeKey = "Seriousness", Value = "Death", SelectionKey = "3"},
                new SelectionDataItem {AttributeKey = "Seriousness", Value = "Initial or prolonged hospitalization", SelectionKey = "4"},
                new SelectionDataItem {AttributeKey = "Seriousness", Value = "Life threatening", SelectionKey = "5"},
                new SelectionDataItem {AttributeKey = "Seriousness", Value = "A medically important event", SelectionKey = "6"},
				new SelectionDataItem {AttributeKey = "Effect OF Dechallenge (D) & Rechallenge (R)", Value = "", SelectionKey = "0"},
                new SelectionDataItem {AttributeKey = "Effect OF Dechallenge (D) & Rechallenge (R)", Value = "Not Applicable", SelectionKey = "1"},
                new SelectionDataItem {AttributeKey = "Effect OF Dechallenge (D) & Rechallenge (R)", Value = "D - AE improved/ resolved when medicine dose reduced/interrupted /withdrawn", SelectionKey = "2"},
                new SelectionDataItem {AttributeKey = "Effect OF Dechallenge (D) & Rechallenge (R)", Value = "D - AE did not improve/ resolve when medicine dose reduced/interrupted /withdrawn", SelectionKey = "3"},
                new SelectionDataItem {AttributeKey = "Effect OF Dechallenge (D) & Rechallenge (R)", Value = "D - Unknown", SelectionKey = "4"},
                new SelectionDataItem {AttributeKey = "Effect OF Dechallenge (D) & Rechallenge (R)", Value = "R - patient not re-exposed to the medicine", SelectionKey = "5"},
                new SelectionDataItem {AttributeKey = "Effect OF Dechallenge (D) & Rechallenge (R)", Value = "R - AE recurred on medicine re-administration/dose increase", SelectionKey = "6"},
                new SelectionDataItem {AttributeKey = "Effect OF Dechallenge (D) & Rechallenge (R)", Value = "R - AE did not recur on medicine re-administration/dose increase", SelectionKey = "7"},
                new SelectionDataItem {AttributeKey = "Effect OF Dechallenge (D) & Rechallenge (R)", Value = "R - Unknown", SelectionKey = "8"},
				new SelectionDataItem {AttributeKey = "Was the AE attributed to one or more drugs?", Value = "", SelectionKey = "0"},
                new SelectionDataItem {AttributeKey = "Was the AE attributed to one or more drugs?", Value = "Yes", SelectionKey = "1"},
                new SelectionDataItem {AttributeKey = "Was the AE attributed to one or more drugs?", Value = "No", SelectionKey = "2"},
                new SelectionDataItem {AttributeKey = "Was the AE attributed to one or more drugs?", Value = "Unknown", SelectionKey = "3"},
				new SelectionDataItem {AttributeKey = "Was the event reported to national PV?", Value = "", SelectionKey = "0"},
                new SelectionDataItem {AttributeKey = "Was the event reported to national PV?", Value = "Yes", SelectionKey = "1"},
                new SelectionDataItem {AttributeKey = "Was the event reported to national PV?", Value = "No", SelectionKey = "2"},
                new SelectionDataItem {AttributeKey = "Was the event reported to national PV?", Value = "Unknown", SelectionKey = "3"},
				new SelectionDataItem {AttributeKey = "Reason For Stopping", Value = "", SelectionKey = "0"},
                new SelectionDataItem {AttributeKey = "Reason For Stopping", Value = "Adverse Event", SelectionKey = "1"},
                new SelectionDataItem {AttributeKey = "Reason For Stopping", Value = "Cost", SelectionKey = "2"},
                new SelectionDataItem {AttributeKey = "Reason For Stopping", Value = "Course Completed", SelectionKey = "3"},
                new SelectionDataItem {AttributeKey = "Reason For Stopping", Value = "Cured", SelectionKey = "4"},
                new SelectionDataItem {AttributeKey = "Reason For Stopping", Value = "Lost To Follow-up", SelectionKey = "5"},
                new SelectionDataItem {AttributeKey = "Reason For Stopping", Value = "Medicine Out of Stock", SelectionKey = "6"},
                new SelectionDataItem {AttributeKey = "Reason For Stopping", Value = "No Longer Needed", SelectionKey = "7"},
                new SelectionDataItem {AttributeKey = "Reason For Stopping", Value = "Patient Died", SelectionKey = "8"},
                new SelectionDataItem {AttributeKey = "Reason For Stopping", Value = "Patient Withdrew Consent", SelectionKey = "9"},
                new SelectionDataItem {AttributeKey = "Reason For Stopping", Value = "Planned Medication Change", SelectionKey = "10"},
                new SelectionDataItem {AttributeKey = "Reason For Stopping", Value = "Poor Adherence", SelectionKey = "11"},
                new SelectionDataItem {AttributeKey = "Reason For Stopping", Value = "Pregnancy", SelectionKey = "12"},
                new SelectionDataItem {AttributeKey = "Reason For Stopping", Value = "Treatment Failure", SelectionKey = "13"},
                new SelectionDataItem {AttributeKey = "Reason For Stopping", Value = "Not Applicable", SelectionKey = "14"},
				new SelectionDataItem {AttributeKey = "Clinician action taken with regard to medicine if related to AE", Value = "", SelectionKey = "0"},
                new SelectionDataItem {AttributeKey = "Clinician action taken with regard to medicine if related to AE", Value = "Dose not changed", SelectionKey = "1"},
                new SelectionDataItem {AttributeKey = "Clinician action taken with regard to medicine if related to AE", Value = "Dose reduced", SelectionKey = "2"},
                new SelectionDataItem {AttributeKey = "Clinician action taken with regard to medicine if related to AE", Value = "Drug interrupted", SelectionKey = "3"},
                new SelectionDataItem {AttributeKey = "Clinician action taken with regard to medicine if related to AE", Value = "Drug withdrawn", SelectionKey = "4"},
                new SelectionDataItem {AttributeKey = "Clinician action taken with regard to medicine if related to AE", Value = "Not applicable", SelectionKey = "5"},
				new SelectionDataItem {AttributeKey = "Type of Indication", Value = "", SelectionKey = "0"},
                new SelectionDataItem {AttributeKey = "Type of Indication", Value = "Primary", SelectionKey = "1"},
                new SelectionDataItem {AttributeKey = "Type of Indication", Value = "Pre-existing condition", SelectionKey = "2"},
                new SelectionDataItem {AttributeKey = "Type of Indication", Value = "Treat AE", SelectionKey = "3"},
                new SelectionDataItem {AttributeKey = "Gender", Value = "", SelectionKey = "0"},
                new SelectionDataItem {AttributeKey = "Gender", Value = "Male", SelectionKey = "1"},
                new SelectionDataItem {AttributeKey = "Gender", Value = "Female", SelectionKey = "2"},
                new SelectionDataItem {AttributeKey = "Marital Status", Value = "", SelectionKey = "0"},
                new SelectionDataItem {AttributeKey = "Marital Status", Value = "Single", SelectionKey = "1"},
                new SelectionDataItem {AttributeKey = "Marital Status", Value = "Married", SelectionKey = "2"},
                new SelectionDataItem {AttributeKey = "Marital Status", Value = "Divorced", SelectionKey = "3"},
                new SelectionDataItem {AttributeKey = "Marital Status", Value = "Widowed", SelectionKey = "4"},
                new SelectionDataItem {AttributeKey = "Marital Status", Value = "Legally Seperated", SelectionKey = "5"},
                new SelectionDataItem {AttributeKey = "Employment Status", Value = "", SelectionKey = "0"},
                new SelectionDataItem {AttributeKey = "Employment Status", Value = "Employed", SelectionKey = "1"},
                new SelectionDataItem {AttributeKey = "Employment Status", Value = "Unemployed", SelectionKey = "2"},
                new SelectionDataItem {AttributeKey = "Employment Status", Value = "Student", SelectionKey = "3"},
                new SelectionDataItem {AttributeKey = "Employment Status", Value = "N/A", SelectionKey = "4"},
                new SelectionDataItem {AttributeKey = "Language", Value = "", SelectionKey = "0"},
                new SelectionDataItem {AttributeKey = "Language", Value = "English", SelectionKey = "1"},
                new SelectionDataItem {AttributeKey = "Language", Value = "French", SelectionKey = "2"},
                new SelectionDataItem {AttributeKey = "Still On Medication", Value = "", SelectionKey = "0"},
                new SelectionDataItem {AttributeKey = "Still On Medication", Value = "Yes", SelectionKey = "1"},
                new SelectionDataItem {AttributeKey = "Still On Medication", Value = "No", SelectionKey = "2"},
                new SelectionDataItem {AttributeKey = "Condition Ongoing", Value = "", SelectionKey = "0"},
                new SelectionDataItem {AttributeKey = "Condition Ongoing", Value = "Yes", SelectionKey = "1"},
                new SelectionDataItem {AttributeKey = "Condition Ongoing", Value = "No", SelectionKey = "2"},
                new SelectionDataItem {AttributeKey = "Identity Type", Value = "", SelectionKey = "0"},
                new SelectionDataItem {AttributeKey = "Identity Type", Value = "National identity", SelectionKey = "1"},
                new SelectionDataItem {AttributeKey = "Identity Type", Value = "Passport number", SelectionKey = "2"},
                new SelectionDataItem {AttributeKey = "Identity Type", Value = "Work permit number", SelectionKey = "3"},
                new SelectionDataItem {AttributeKey = "Route", Value = "", SelectionKey = "0"},
                new SelectionDataItem {AttributeKey = "Route", Value = "Auricular (otic)", SelectionKey = "1"},
                new SelectionDataItem {AttributeKey = "Route", Value = "Buccal", SelectionKey = "2"},
                new SelectionDataItem {AttributeKey = "Route", Value = "Cutaneous", SelectionKey = "3"},
                new SelectionDataItem {AttributeKey = "Route", Value = "Dental", SelectionKey = "4"},
                new SelectionDataItem {AttributeKey = "Route", Value = "Endocervical", SelectionKey = "5"},
                new SelectionDataItem {AttributeKey = "Route", Value = "Endosinusial", SelectionKey = "6"},
                new SelectionDataItem {AttributeKey = "Route", Value = "Endotracheal", SelectionKey = "7"},
                new SelectionDataItem {AttributeKey = "Route", Value = "Epidural", SelectionKey = "8"},
                new SelectionDataItem {AttributeKey = "Route", Value = "Extra-amniotic", SelectionKey = "9"},
                new SelectionDataItem {AttributeKey = "Route", Value = "Hemodialysis", SelectionKey = "10"},
                new SelectionDataItem {AttributeKey = "Route", Value = "Intra corpus cavernosum", SelectionKey = "11"},
                new SelectionDataItem {AttributeKey = "Route", Value = "Intra-amniotic", SelectionKey = "12"},
                new SelectionDataItem {AttributeKey = "Route", Value = "Intra-arterial", SelectionKey = "13"},
                new SelectionDataItem {AttributeKey = "Route", Value = "Intra-articular", SelectionKey = "14"},
                new SelectionDataItem {AttributeKey = "Route", Value = "Intra-uterine", SelectionKey = "15"},
                new SelectionDataItem {AttributeKey = "Route", Value = "Intracardiac", SelectionKey = "16"},
                new SelectionDataItem {AttributeKey = "Route", Value = "Intracavernous", SelectionKey = "17"},
                new SelectionDataItem {AttributeKey = "Route", Value = "Intracerebral", SelectionKey = "18"},
                new SelectionDataItem {AttributeKey = "Route", Value = "Intracervical", SelectionKey = "19"},
                new SelectionDataItem {AttributeKey = "Route", Value = "Intracisternal", SelectionKey = "20"},
                new SelectionDataItem {AttributeKey = "Route", Value = "Intracorneal", SelectionKey = "21"},
                new SelectionDataItem {AttributeKey = "Route", Value = "Intracoronary", SelectionKey = "22"},
                new SelectionDataItem {AttributeKey = "Route", Value = "Intradermal", SelectionKey = "23"},
                new SelectionDataItem {AttributeKey = "Route", Value = "Intradiscal (intraspinal)", SelectionKey = "24"},
                new SelectionDataItem {AttributeKey = "Route", Value = "Intrahepatic", SelectionKey = "25"},
                new SelectionDataItem {AttributeKey = "Route", Value = "Intralesional", SelectionKey = "26"},
                new SelectionDataItem {AttributeKey = "Route", Value = "Intralymphatic", SelectionKey = "27"},
                new SelectionDataItem {AttributeKey = "Route", Value = "Intramedullar (bone marrow)", SelectionKey = "28"},
                new SelectionDataItem {AttributeKey = "Route", Value = "Intrameningeal", SelectionKey = "29"},
                new SelectionDataItem {AttributeKey = "Route", Value = "Intramuscular", SelectionKey = "30"},
                new SelectionDataItem {AttributeKey = "Route", Value = "Intraocular", SelectionKey = "31"},
                new SelectionDataItem {AttributeKey = "Route", Value = "Intrapericardial", SelectionKey = "32"},
                new SelectionDataItem {AttributeKey = "Route", Value = "Intraperitoneal", SelectionKey = "33"},
                new SelectionDataItem {AttributeKey = "Route", Value = "Intrapleural", SelectionKey = "34"},
                new SelectionDataItem {AttributeKey = "Route", Value = "Intrasynovial", SelectionKey = "35"},
                new SelectionDataItem {AttributeKey = "Route", Value = "Intratumor", SelectionKey = "36"},
                new SelectionDataItem {AttributeKey = "Route", Value = "Intrathecal", SelectionKey = "37"},
                new SelectionDataItem {AttributeKey = "Route", Value = "Intrathoracic", SelectionKey = "38"},
                new SelectionDataItem {AttributeKey = "Route", Value = "Intratracheal", SelectionKey = "39"},
                new SelectionDataItem {AttributeKey = "Route", Value = "Intravenous bolus", SelectionKey = "40"},
                new SelectionDataItem {AttributeKey = "Route", Value = "Intravenous drip", SelectionKey = "41"},
                new SelectionDataItem {AttributeKey = "Route", Value = "Intravenous (not otherwise specified)", SelectionKey = "42"},
                new SelectionDataItem {AttributeKey = "Route", Value = "Intravesical", SelectionKey = "43"},
                new SelectionDataItem {AttributeKey = "Route", Value = "Iontophoresis", SelectionKey = "44"},
                new SelectionDataItem {AttributeKey = "Route", Value = "Nasal", SelectionKey = "45"},
                new SelectionDataItem {AttributeKey = "Route", Value = "Occlusive dressing technique", SelectionKey = "46"},
                new SelectionDataItem {AttributeKey = "Route", Value = "Ophthalmic", SelectionKey = "47"},
                new SelectionDataItem {AttributeKey = "Route", Value = "Oral", SelectionKey = "48"},
                new SelectionDataItem {AttributeKey = "Route", Value = "Oropharingeal", SelectionKey = "49"},
                new SelectionDataItem {AttributeKey = "Route", Value = "Other", SelectionKey = "50"},
                new SelectionDataItem {AttributeKey = "Route", Value = "Parenteral 051", SelectionKey = "51"},
                new SelectionDataItem {AttributeKey = "Route", Value = "Periarticular", SelectionKey = "52"},
                new SelectionDataItem {AttributeKey = "Route", Value = "Perineural", SelectionKey = "53"},
                new SelectionDataItem {AttributeKey = "Route", Value = "Rectal", SelectionKey = "54"},
                new SelectionDataItem {AttributeKey = "Route", Value = "Respiratory (inhalation)", SelectionKey = "55"},
                new SelectionDataItem {AttributeKey = "Route", Value = "Retrobulbar", SelectionKey = "56"},
                new SelectionDataItem {AttributeKey = "Route", Value = "Sunconjunctival", SelectionKey = "57"},
                new SelectionDataItem {AttributeKey = "Route", Value = "Subcutaneous", SelectionKey = "58"},
                new SelectionDataItem {AttributeKey = "Route", Value = "Subdermal", SelectionKey = "59"},
                new SelectionDataItem {AttributeKey = "Route", Value = "Sublingual", SelectionKey = "60"},
                new SelectionDataItem {AttributeKey = "Route", Value = "Topical", SelectionKey = "61"},
                new SelectionDataItem {AttributeKey = "Route", Value = "Transdermal", SelectionKey = "62"},
                new SelectionDataItem {AttributeKey = "Route", Value = "Transmammary", SelectionKey = "63"},
                new SelectionDataItem {AttributeKey = "Route", Value = "Transplacental", SelectionKey = "64"},
                new SelectionDataItem {AttributeKey = "Route", Value = "Unknown", SelectionKey = "65"},
                new SelectionDataItem {AttributeKey = "Route", Value = "Urethral", SelectionKey = "66"},
                new SelectionDataItem {AttributeKey = "Route", Value = "Vaginal", SelectionKey = "67"},
                new SelectionDataItem {AttributeKey = "Frequency in days per week", Value = "", SelectionKey = "0"},
                new SelectionDataItem {AttributeKey = "Frequency in days per week", Value = "1", SelectionKey = "1"},
                new SelectionDataItem {AttributeKey = "Frequency in days per week", Value = "2", SelectionKey = "2"},
                new SelectionDataItem {AttributeKey = "Frequency in days per week", Value = "3", SelectionKey = "3"},
                new SelectionDataItem {AttributeKey = "Frequency in days per week", Value = "4", SelectionKey = "4"},
                new SelectionDataItem {AttributeKey = "Frequency in days per week", Value = "5", SelectionKey = "5"},
                new SelectionDataItem {AttributeKey = "Frequency in days per week", Value = "6", SelectionKey = "6"},
                new SelectionDataItem {AttributeKey = "Frequency in days per week", Value = "7", SelectionKey = "7"},
                new SelectionDataItem {AttributeKey = "Autopsy Done", Value = "", SelectionKey = "0"},
                new SelectionDataItem {AttributeKey = "Autopsy Done", Value = "Yes", SelectionKey = "1"},
                new SelectionDataItem {AttributeKey = "Autopsy Done", Value = "No", SelectionKey = "2"}
            };

            context.SelectionDataItems.AddOrUpdate(s => new { s.AttributeKey, s.Value }, selectionDataItems);
            context.SaveChanges();
        }

        private static void CreateFacilityTypes(PVIMSDbContext context)
        {
            var types = new[]
			{
				new FacilityType { Description = "Unknown" },
				new FacilityType { Description = "Hospital" },
				new FacilityType { Description = "CHC" },
                new FacilityType { Description = "PHC" }
            };

            context.FacilityTypes.AddOrUpdate(ft => ft.Description, types);
            context.SaveChanges();
        }

        private static void CreateFieldTypes(PVIMSDbContext context)
        {
            var fieldTypes = new[]
			{
				new FieldType {Description = "Listbox"},
				new FieldType {Description = "DropDownList"},
				new FieldType {Description = "AlphaNumericTextbox"},
				new FieldType {Description = "NumericTextbox"},
				new FieldType {Description = "YesNo"},
                new FieldType {Description = "Date"},
                new FieldType {Description = "Table"},
                new FieldType {Description = "System"}
			};

            context.FieldTypes.AddOrUpdate(ft => ft.Description, fieldTypes);
            context.SaveChanges();
        }

        private static void CreateMetaTypes(PVIMSDbContext context)
        {
            var metaTableTypes = new[]
			{
				new MetaTableType { metatabletype_guid = Guid.NewGuid(), Description = "Core" },
                new MetaTableType { metatabletype_guid = Guid.NewGuid(), Description = "CoreChild" },
                new MetaTableType { metatabletype_guid = Guid.NewGuid(), Description = "Child" },
                new MetaTableType { metatabletype_guid = Guid.NewGuid(), Description = "History" },
                new MetaTableType { metatabletype_guid = Guid.NewGuid(), Description = "Lookup" }
			};

            context.MetaTableTypes.AddOrUpdate(mt => mt.Description, metaTableTypes);
            context.SaveChanges();

            var metaColumnTypes = new[]
			{
				new MetaColumnType { metacolumntype_guid = Guid.NewGuid(), Description = "bigint" },
				new MetaColumnType { metacolumntype_guid = Guid.NewGuid(), Description = "binary" },
				new MetaColumnType { metacolumntype_guid = Guid.NewGuid(), Description = "bit" },
				new MetaColumnType { metacolumntype_guid = Guid.NewGuid(), Description = "char" },
				new MetaColumnType { metacolumntype_guid = Guid.NewGuid(), Description = "date" },
				new MetaColumnType { metacolumntype_guid = Guid.NewGuid(), Description = "datetime" },
				new MetaColumnType { metacolumntype_guid = Guid.NewGuid(), Description = "decimal" },
				new MetaColumnType { metacolumntype_guid = Guid.NewGuid(), Description = "image" },
				new MetaColumnType { metacolumntype_guid = Guid.NewGuid(), Description = "int" },
				new MetaColumnType { metacolumntype_guid = Guid.NewGuid(), Description = "nchar" },
				new MetaColumnType { metacolumntype_guid = Guid.NewGuid(), Description = "nvarchar" },
				new MetaColumnType { metacolumntype_guid = Guid.NewGuid(), Description = "smallint" },
				new MetaColumnType { metacolumntype_guid = Guid.NewGuid(), Description = "time" },
				new MetaColumnType { metacolumntype_guid = Guid.NewGuid(), Description = "tinyint" },
				new MetaColumnType { metacolumntype_guid = Guid.NewGuid(), Description = "uniqueidentifier" },
				new MetaColumnType { metacolumntype_guid = Guid.NewGuid(), Description = "varbinary" },
				new MetaColumnType { metacolumntype_guid = Guid.NewGuid(), Description = "varchar" }
			};

            context.MetaColumnTypes.AddOrUpdate(mt => mt.Description, metaColumnTypes);
            context.SaveChanges();

            var metaWidgetTypes = new[]
			{
                new MetaWidgetType { metawidgettype_guid = Guid.NewGuid(), Description = "General" },
                new MetaWidgetType { metawidgettype_guid = Guid.NewGuid(), Description = "Wiki" },
                new MetaWidgetType { metawidgettype_guid = Guid.NewGuid(), Description = "ItemList" }
            };

            context.MetaWidgetTypes.AddOrUpdate(mt => mt.Description, metaWidgetTypes);
            context.SaveChanges();
        }

        private static void CreateConfigValues(PVIMSDbContext context)
        {
            var configs = new[]
			{
				new Config { ConfigType = ConfigType.E2BVersion, ConfigValue = "E2B(R2) ICH Report" },
				new Config { ConfigType = ConfigType.WebServiceSubscriberList, ConfigValue = "NOT SPECIFIED" },
				new Config { ConfigType = ConfigType.AssessmentScale, ConfigValue = "Both Scales" },
                new Config { ConfigType = ConfigType.MedDRAVersion, ConfigValue = "23.0" },
                new Config { ConfigType = ConfigType.ReportInstanceNewAlertCount, ConfigValue = "0" },
                new Config { ConfigType = ConfigType.MedicationOnsetCheckPeriodWeeks, ConfigValue = "5" },
                new Config { ConfigType = ConfigType.MetaDataLastUpdated, ConfigValue = DateTime.Now.ToString("yyyy-MM-dd HH:mm") },
                new Config { ConfigType = ConfigType.PharmadexLink, ConfigValue = "NOT SPECIFIED" }
            };

            context.Configs.AddOrUpdate(c => c.ConfigType, configs);
            context.SaveChanges();
        }

        private static void CreateContactTypes(PVIMSDbContext context)
        {
            var types = new[]
			{
                new SiteContactDetail { ContactType = ContactType.RegulatoryAuthority, ContactFirstName = "Not", ContactSurname = "Specified", StreetAddress = "None", City = "None", OrganisationName = "None" },
                new SiteContactDetail { ContactType = ContactType.ReportingAuthority, ContactFirstName = "Uppsala", ContactSurname = "Monitoring Centre", StreetAddress = "Bredgrand 7B", City = "Uppsala", State = "None", PostCode = "75320", ContactEmail = "info@who-umc.org", ContactNumber = "18656060", CountryCode = "46", OrganisationName = "UMC" }
            };

            context.SiteContactDetails.AddOrUpdate(c => c.ContactType, types);
            context.SaveChanges();
        }

        private static void CreateContextTypes(PVIMSDbContext context)
        {
            var contextTypes = new[]
			{
				new ContextType {Description = "Encounter"},
				new ContextType {Description = "Patient"},
				new ContextType {Description = "Pregnancy"},
                new ContextType {Description = "Global"},
                new ContextType {Description = "PatientClinicalEvent"},
                new ContextType {Description = "DatasetInstance"}
			};

            context.ContextTypes.AddOrUpdate(ct => ct.Description, contextTypes);
            context.SaveChanges();
        }

        private static void CreatePostDeploymentScripts(PVIMSDbContext context)
        {
            var postDeployments = new[]
            {
               new PostDeployment { ScriptGuid = Guid.NewGuid(),   ScriptFileName = "dbo.spGenerateRiskFactors.sql", ScriptDescription="Analysis - Generate risk factors",RunRank = 1},
               new PostDeployment { ScriptGuid = Guid.NewGuid(),   ScriptFileName = "dbo.spGeneratePatientListCondition.sql", ScriptDescription="Analysis - Generate patient conditions",RunRank = 2},
               new PostDeployment { ScriptGuid = Guid.NewGuid(),   ScriptFileName = "dbo.spGeneratePatientListCohort.sql", ScriptDescription="Analysis - Generate patient cohorts",RunRank = 3},
               new PostDeployment { ScriptGuid = Guid.NewGuid(),   ScriptFileName = "dbo.spGenerateDrugList.sql", ScriptDescription="Analysis - Generate drug list",RunRank = 4},
               new PostDeployment { ScriptGuid = Guid.NewGuid(),   ScriptFileName = "dbo.spGenerateContingency.sql", ScriptDescription="Analysis - Generate contingency",RunRank = 5},
               new PostDeployment { ScriptGuid = Guid.NewGuid(),   ScriptFileName = "dbo.spGenerateAnalysis.sql", ScriptDescription="Analysis - Generate analysis",RunRank = 6},
               new PostDeployment { ScriptGuid = Guid.NewGuid(),   ScriptFileName = "dbo.CleanJsonForString.sql", ScriptDescription="JSON String Handler Function",RunRank = 7},
               //new PostDeployment { ScriptGuid = Guid.NewGuid(),   ScriptFileName = "dbo.SeedMedications.sql", ScriptDescription="Seed Medications",RunRank = 8},
               new PostDeployment { ScriptGuid = Guid.NewGuid(),   ScriptFileName = "dbo.SeedMeta.sql", ScriptDescription="Seed META",RunRank = 9},
               new PostDeployment { ScriptGuid = Guid.NewGuid(),   ScriptFileName = "dbo.SeedMedDRA_1.sql", ScriptDescription="Seed MedDRA 10000",RunRank = 10},
               new PostDeployment { ScriptGuid = Guid.NewGuid(),   ScriptFileName = "dbo.SeedMedDRA_2.sql", ScriptDescription="Seed MedDRA 20000",RunRank = 11},
               new PostDeployment { ScriptGuid = Guid.NewGuid(),   ScriptFileName = "dbo.SeedMedDRA_3.sql", ScriptDescription="Seed MedDRA 30000",RunRank = 12},
               new PostDeployment { ScriptGuid = Guid.NewGuid(),   ScriptFileName = "dbo.SeedMedDRA_4.sql", ScriptDescription="Seed MedDRA 40000",RunRank = 13},
               new PostDeployment { ScriptGuid = Guid.NewGuid(),   ScriptFileName = "dbo.SeedMedDRA_5.sql", ScriptDescription="Seed MedDRA 50000",RunRank = 14},
               new PostDeployment { ScriptGuid = Guid.NewGuid(),   ScriptFileName = "dbo.SeedMedDRA_6.sql", ScriptDescription="Seed MedDRA 60000",RunRank = 15},
               new PostDeployment { ScriptGuid = Guid.NewGuid(),   ScriptFileName = "dbo.SeedMedDRA_7.sql", ScriptDescription="Seed MedDRA 70000",RunRank = 16},
               new PostDeployment { ScriptGuid = Guid.NewGuid(),   ScriptFileName = "dbo.SeedMedDRA_8.sql", ScriptDescription="Seed MedDRA 80000",RunRank = 17},
               new PostDeployment { ScriptGuid = Guid.NewGuid(),   ScriptFileName = "dbo.SeedMedDRA_9.sql", ScriptDescription="Seed MedDRA 90000",RunRank = 18},
               new PostDeployment { ScriptGuid = Guid.NewGuid(),   ScriptFileName = "dbo.SeedMedDRA_10.sql", ScriptDescription="Seed MedDRA 100000",RunRank = 19},
               new PostDeployment { ScriptGuid = Guid.NewGuid(),   ScriptFileName = "dbo.SeedConditions.sql", ScriptDescription="Seed Conditions",RunRank = 20},
               //new PostDeployment { ScriptGuid = Guid.NewGuid(),   ScriptFileName = "dbo.Datapack.Chronic.sql", ScriptDescription="Seed Chronic Dataset",RunRank = 21},
               new PostDeployment { ScriptGuid = Guid.NewGuid(),   ScriptFileName = "dbo.Datapack.Spontaneous.sql", ScriptDescription="Seed Spontaneous Dataset",RunRank = 22},
               new PostDeployment { ScriptGuid = Guid.NewGuid(),   ScriptFileName = "dbo.Datapack.E2BR2.sql", ScriptDescription="Seed E2B R2 Dataset",RunRank = 23},
               new PostDeployment { ScriptGuid = Guid.NewGuid(),   ScriptFileName = "dbo.SeedWorkFlow.sql", ScriptDescription="Seed Work Flow Processes",RunRank = 24},
               new PostDeployment { ScriptGuid = Guid.NewGuid(),   ScriptFileName = "dbo.SeedRiskFactor.sql", ScriptDescription="Seed Risk Factors",RunRank = 25},
            };

            context.PostDeployments.AddOrUpdate(r => r.ScriptFileName, postDeployments);
            context.SaveChanges();
        }
    }
}
