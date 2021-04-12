using Microsoft.AspNetCore.Hosting;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Retry;
using PVIMS.Core.Entities;
using PVIMS.Core.Entities.Accounts;
using PVIMS.Core.ValueTypes;
using PVIMS.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PVIMS.API.Infrastructure
{
    public class PVIMSContextSeed
    {
        public async Task SeedAsync(PVIMSDbContext context, IWebHostEnvironment env, IOptions<PVIMSSettings> settings, ILogger<PVIMSContextSeed> logger)
        {
            var policy = CreatePolicy(logger, nameof(PVIMSContextSeed));

            await policy.ExecuteAsync(async () =>
            {
                var contentRootPath = env.ContentRootPath;

                using (context)
                {
                    context.Database.Migrate();

                    context.Roles.AddRange(PrepareRoles(context));
                    await context.SaveEntitiesAsync();

                    context.Users.AddRange(PrepareAdminUser(context));
                    await context.SaveEntitiesAsync();

                    context.AttachmentTypes.AddRange(PrepareAttachmentTypes(context));
                    context.CareEvents.AddRange(PrepareCareEvents(context));
                    context.Configs.AddRange(PrepareConfigs(context));
                    context.ContextTypes.AddRange(PrepareContextTypes(context));
                    context.DatasetElementTypes.AddRange(PrepareDatasetElementTypes(context));
                    context.FacilityTypes.AddRange(PrepareFacilityTypes(context));
                    context.FieldTypes.AddRange(PrepareFieldTypes(context));
                    context.LabResults.AddRange(PrepareLabResults(context));
                    context.LabTestUnits.AddRange(PrepareLabTestUnits(context));
                    context.MetaColumnTypes.AddRange(PrepareMetaColumnTypes(context));
                    context.MetaTableTypes.AddRange(PrepareMetaTableTypes(context));
                    context.MetaWidgetTypes.AddRange(PrepareMetaWidgetTypes(context));
                    context.Outcomes.AddRange(PrepareOutcomes(context));
                    context.PatientStatuses.AddRange(PreparePatientStatus(context));
                    context.Priorities.AddRange(PreparePriorities(context));
                    context.SiteContactDetails.AddRange(PrepareSiteContactDetails(context));
                    context.TreatmentOutcomes.AddRange(PrepareTreatmentOutcomes(context));

                    await context.SaveEntitiesAsync();
                }
            });
        }

        private IEnumerable<Role> PrepareRoles(PVIMSDbContext context)
        {
            List<Role> roles = new List<Role>();

            if (!context.Roles.Any(ce => ce.Key == "Admin"))
                roles.Add(new Role { Name = "Administrator", Key = "Admin" } );

            if (!context.Roles.Any(ce => ce.Key == "RegClerk"))
                roles.Add(new Role { Name = "Registration Clerk", Key = "RegClerk" });

            if (!context.Roles.Any(ce => ce.Key == "DataCap"))
                roles.Add(new Role { Name = "Data Capturer", Key = "DataCap" });

            if (!context.Roles.Any(ce => ce.Key == "Clinician"))
                roles.Add(new Role { Name = "Clinician", Key = "Clinician" });

            if (!context.Roles.Any(ce => ce.Key == "Analyst"))
                roles.Add(new Role { Name = "Analytics", Key = "Analyst" });

            if (!context.Roles.Any(ce => ce.Key == "Reporter"))
                roles.Add(new Role { Name = "Reporter", Key = "Reporter" });

            if (!context.Roles.Any(ce => ce.Key == "Publisher"))
                roles.Add(new Role { Name = "Publisher", Key = "Publisher" });

            if (!context.Roles.Any(ce => ce.Key == "ReporterAdmin"))
                roles.Add(new Role { Name = "Reporter Administrator", Key = "ReporterAdmin" });

            if (!context.Roles.Any(ce => ce.Key == "PublisherAdmin"))
                roles.Add(new Role { Name = "Publisher Administrator", Key = "PublisherAdmin" });

            return roles;
        }

        private IEnumerable<User> PrepareAdminUser(PVIMSDbContext context)
        {
            List<User> users = new List<User>();

            if (!context.Users.Any(ce => ce.UserName == "Admin"))
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

                adminUser.Roles.Add(new UserRole { User = adminUser, Role = context.Roles.SingleOrDefault(r => r.Key == "Admin") });
                adminUser.Roles.Add(new UserRole { User = adminUser, Role = context.Roles.SingleOrDefault(r => r.Key == "DataCap") });
                adminUser.Roles.Add(new UserRole { User = adminUser, Role = context.Roles.SingleOrDefault(r => r.Key == "Analyst") });
                adminUser.Roles.Add(new UserRole { User = adminUser, Role = context.Roles.SingleOrDefault(r => r.Key == "Reporter") });
                adminUser.Roles.Add(new UserRole { User = adminUser, Role = context.Roles.SingleOrDefault(r => r.Key == "Publisher") });
                adminUser.Roles.Add(new UserRole { User = adminUser, Role = context.Roles.SingleOrDefault(r => r.Key == "ReporterAdmin") });
                adminUser.Roles.Add(new UserRole { User = adminUser, Role = context.Roles.SingleOrDefault(r => r.Key == "PublisherAdmin") });

                users.Add(adminUser);
            }

            return users;
        }

        private IEnumerable<PatientStatus> PreparePatientStatus(PVIMSDbContext context)
        {
            List<PatientStatus> patientStatuses = new List<PatientStatus>();

            if (!context.PatientStatuses.Any(ce => ce.Description == "Active"))
            {
                patientStatuses.Add(new PatientStatus { Description = "Active" });
            }

            if (!context.PatientStatuses.Any(ce => ce.Description == "Suspended"))
            {
                patientStatuses.Add(new PatientStatus { Description = "Suspended" });
            }

            if (!context.PatientStatuses.Any(ce => ce.Description == "Transferred Out"))
            {
                patientStatuses.Add(new PatientStatus { Description = "Transferred Out" });
            }

            if (!context.PatientStatuses.Any(ce => ce.Description == "Died"))
            {
                patientStatuses.Add(new PatientStatus { Description = "Died" });
            }

            return patientStatuses;
        }

        private IEnumerable<AttachmentType> PrepareAttachmentTypes(PVIMSDbContext context)
        {
            List<AttachmentType> attachmentTypes = new List<AttachmentType>();

            if (!context.AttachmentTypes.Any(ce => ce.Key == "doc"))
                attachmentTypes.Add(new AttachmentType { Description = "MS Word 2003-2007 Document", Key = "doc" });

            if (!context.AttachmentTypes.Any(ce => ce.Key == "xls"))
                attachmentTypes.Add(new AttachmentType { Description = "MS Excel 2003-2007 Document", Key = "xls" });

            if (!context.AttachmentTypes.Any(ce => ce.Key == "docx"))
                attachmentTypes.Add(new AttachmentType { Description = "MS Word Document", Key = "docx" });

            if (!context.AttachmentTypes.Any(ce => ce.Key == "xlsx"))
                attachmentTypes.Add(new AttachmentType { Description = "MS Excel Document", Key = "xlsx" });

            if (!context.AttachmentTypes.Any(ce => ce.Key == "pdf"))
                attachmentTypes.Add(new AttachmentType { Description = "Portable Document Format", Key = "pdf" });

            if (!context.AttachmentTypes.Any(ce => ce.Key == "jpg"))
                attachmentTypes.Add(new AttachmentType { Description = "Image | JPEG", Key = "jpg" });

            if (!context.AttachmentTypes.Any(ce => ce.Key == "jpeg"))
                attachmentTypes.Add(new AttachmentType { Description = "Image | JPEG", Key = "jpeg" });

            if (!context.AttachmentTypes.Any(ce => ce.Key == "png"))
                attachmentTypes.Add(new AttachmentType { Description = "Image | PNG", Key = "png" });

            if (!context.AttachmentTypes.Any(ce => ce.Key == "bmp"))
                attachmentTypes.Add(new AttachmentType { Description = "Image | BMP", Key = "bmp" });

            if (!context.AttachmentTypes.Any(ce => ce.Key == "xml"))
                attachmentTypes.Add(new AttachmentType { Description = "XML Document", Key = "xml" });

            return attachmentTypes;
        }

        private IEnumerable<Outcome> PrepareOutcomes(PVIMSDbContext context)
        {
            List<Outcome> outcomes = new List<Outcome>();

            if (!context.Outcomes.Any(ce => ce.Description == "Recovered/Resolved"))
                outcomes.Add(new Outcome { Description = "Recovered/Resolved" });

            if (!context.Outcomes.Any(ce => ce.Description == "Recovered/Resolved With Sequelae"))
                outcomes.Add(new Outcome { Description = "Recovered/Resolved With Sequelae" });

            if (!context.Outcomes.Any(ce => ce.Description == "Recovering/Resolving"))
                outcomes.Add(new Outcome { Description = "Recovering/Resolving" });

            if (!context.Outcomes.Any(ce => ce.Description == "Not Recovered/Not Resolved"))
                outcomes.Add(new Outcome { Description = "Not Recovered/Not Resolved" });

            if (!context.Outcomes.Any(ce => ce.Description == "Fatal"))
                outcomes.Add(new Outcome { Description = "Fatal" });

            if (!context.Outcomes.Any(ce => ce.Description == "Unknown"))
                outcomes.Add(new Outcome { Description = "Unknown" });

            return outcomes;
        }

        private IEnumerable<TreatmentOutcome> PrepareTreatmentOutcomes(PVIMSDbContext context)
        {
            List<TreatmentOutcome> treatmentOutcomes = new List<TreatmentOutcome>();

            if (!context.TreatmentOutcomes.Any(ce => ce.Description == "Cured"))
                treatmentOutcomes.Add(new TreatmentOutcome { Description = "Cured" });

            if (!context.TreatmentOutcomes.Any(ce => ce.Description == "Treatment Completed"))
                treatmentOutcomes.Add(new TreatmentOutcome { Description = "Treatment Completed" });

            if (!context.TreatmentOutcomes.Any(ce => ce.Description == "Treatment Failed"))
                treatmentOutcomes.Add(new TreatmentOutcome { Description = "Treatment Failed" });

            if (!context.TreatmentOutcomes.Any(ce => ce.Description == "Died"))
                treatmentOutcomes.Add(new TreatmentOutcome { Description = "Died" });

            if (!context.TreatmentOutcomes.Any(ce => ce.Description == "Lost to Follow-up"))
                treatmentOutcomes.Add(new TreatmentOutcome { Description = "Lost to Follow-up" });

            if (!context.TreatmentOutcomes.Any(ce => ce.Description == "Not Evaluated"))
                treatmentOutcomes.Add(new TreatmentOutcome { Description = "Not Evaluated" });

            return treatmentOutcomes;
        }

        private IEnumerable<Priority> PreparePriorities(PVIMSDbContext context)
        {
            List<Priority> priorities = new List<Priority>();

            if (!context.Priorities.Any(ce => ce.Description == "Not Set"))
                priorities.Add(new Priority { Description = "Not Set" });

            if (!context.Priorities.Any(ce => ce.Description == "Urgent"))
                priorities.Add(new Priority { Description = "Urgent" });

            if (!context.Priorities.Any(ce => ce.Description == "High"))
                priorities.Add(new Priority { Description = "High" });

            if (!context.Priorities.Any(ce => ce.Description == "Medium"))
                priorities.Add(new Priority { Description = "Medium" });

            if (!context.Priorities.Any(ce => ce.Description == "Low"))
                priorities.Add(new Priority { Description = "Low" });

            return priorities;
        }

        private IEnumerable<FacilityType> PrepareFacilityTypes(PVIMSDbContext context)
        {
            List<FacilityType> facilityTypes = new List<FacilityType>();

            if (!context.FacilityTypes.Any(ce => ce.Description == "Unknown"))
                facilityTypes.Add(new FacilityType { Description = "Unknown" });

            if (!context.FacilityTypes.Any(ce => ce.Description == "Hospital"))
                facilityTypes.Add(new FacilityType { Description = "Hospital" });

            if (!context.FacilityTypes.Any(ce => ce.Description == "CHC"))
                facilityTypes.Add(new FacilityType { Description = "CHC" });

            if (!context.FacilityTypes.Any(ce => ce.Description == "PHC"))
                facilityTypes.Add(new FacilityType { Description = "PHC" });

            return facilityTypes;
        }

        private IEnumerable<SiteContactDetail> PrepareSiteContactDetails(PVIMSDbContext context)
        {
            List<SiteContactDetail> siteContactDetails = new List<SiteContactDetail>();

            if (!context.SiteContactDetails.Any(ce => ce.ContactType == ContactType.RegulatoryAuthority))
            {
                var regulatoryAuthorityContactDetail = new SiteContactDetail { ContactType = ContactType.RegulatoryAuthority, ContactFirstName = "Not", ContactSurname = "Specified", StreetAddress = "None", City = "None", OrganisationName = "None" };
                regulatoryAuthorityContactDetail.AuditStamp(context.Users.Single(u => u.UserName == "Admin"));
                siteContactDetails.Add(regulatoryAuthorityContactDetail);
            }

            if (!context.SiteContactDetails.Any(ce => ce.ContactType == ContactType.ReportingAuthority))
            {
                var reportingAuthorityContactDetail = new SiteContactDetail { ContactType = ContactType.ReportingAuthority, ContactFirstName = "Uppsala", ContactSurname = "Monitoring Centre", StreetAddress = "Bredgrand 7B", City = "Uppsala", State = "None", PostCode = "75320", ContactEmail = "info@who-umc.org", ContactNumber = "18656060", CountryCode = "46", OrganisationName = "UMC" };
                reportingAuthorityContactDetail.AuditStamp(context.Users.Single(u => u.UserName == "Admin"));
                siteContactDetails.Add(reportingAuthorityContactDetail);
            }

            return siteContactDetails;
        }

        private IEnumerable<CareEvent> PrepareCareEvents(PVIMSDbContext context)
        {
            List<CareEvent> careEvents = new List<CareEvent>();

            if (!context.CareEvents.Any(ce => ce.Description == "Capture Vitals"))
                careEvents.Add(new CareEvent { Description = "Capture Vitals" });

            if (!context.CareEvents.Any(ce => ce.Description == "Doctor Assessment"))
                careEvents.Add(new CareEvent { Description = "Doctor Assessment" });

            if (!context.CareEvents.Any(ce => ce.Description == "Nurse Assessment"))
                careEvents.Add(new CareEvent { Description = "Nurse Assessment" });

            return careEvents;
        }

        private IEnumerable<DatasetElementType> PrepareDatasetElementTypes(PVIMSDbContext context)
        {
            List<DatasetElementType> datasetElementTypes = new List<DatasetElementType>();

            if (!context.DatasetElementTypes.Any(ce => ce.Description == "Generic"))
                datasetElementTypes.Add(new DatasetElementType { Description = "Generic" });

            return datasetElementTypes;
        }

        private IEnumerable<LabResult> PrepareLabResults(PVIMSDbContext context)
        {
            List<LabResult> labResults = new List<LabResult>();

            if (!context.LabResults.Any(ce => ce.Description == "Positive"))
                labResults.Add(new LabResult { Description = "Positive" });

            if (!context.LabResults.Any(ce => ce.Description == "Negative"))
                labResults.Add(new LabResult { Description = "Negative" });

            if (!context.LabResults.Any(ce => ce.Description == "Borderline"))
                labResults.Add(new LabResult { Description = "Borderline" });

            if (!context.LabResults.Any(ce => ce.Description == "Inconclusive"))
                labResults.Add(new LabResult { Description = "Inconclusive" });

            if (!context.LabResults.Any(ce => ce.Description == "Normal"))
                labResults.Add(new LabResult { Description = "Normal" });

            if (!context.LabResults.Any(ce => ce.Description == "Abnormal"))
                labResults.Add(new LabResult { Description = "Abnormal" });

            if (!context.LabResults.Any(ce => ce.Description == "Seronegative"))
                labResults.Add(new LabResult { Description = "Seronegative" });

            if (!context.LabResults.Any(ce => ce.Description == "Seropositive"))
                labResults.Add(new LabResult { Description = "Seropositive" });

            if (!context.LabResults.Any(ce => ce.Description == "Improved"))
                labResults.Add(new LabResult { Description = "Improved" });

            if (!context.LabResults.Any(ce => ce.Description == "Stable"))
                labResults.Add(new LabResult { Description = "Stable" });

            if (!context.LabResults.Any(ce => ce.Description == "Progressed"))
                labResults.Add(new LabResult { Description = "Progressed" });

            return labResults;
        }

        private IEnumerable<LabTestUnit> PrepareLabTestUnits(PVIMSDbContext context)
        {
            List<LabTestUnit> labTestUnits = new List<LabTestUnit>();

            if (!context.LabTestUnits.Any(ce => ce.Description == "N/A"))
                labTestUnits.Add(new LabTestUnit { Description = "N/A" });

            if (!context.LabTestUnits.Any(ce => ce.Description == "%"))
                labTestUnits.Add(new LabTestUnit { Description = "%" });

            if (!context.LabTestUnits.Any(ce => ce.Description == "% hearing loss left ear"))
                labTestUnits.Add(new LabTestUnit { Description = "% hearing loss left ear" });

            if (!context.LabTestUnits.Any(ce => ce.Description == "% hearing loss right ear"))
                labTestUnits.Add(new LabTestUnit { Description = "% hearing loss right ear" });

            if (!context.LabTestUnits.Any(ce => ce.Description == "µg/dL"))
                labTestUnits.Add(new LabTestUnit { Description = "µg/dL" });

            if (!context.LabTestUnits.Any(ce => ce.Description == "µg/L"))
                labTestUnits.Add(new LabTestUnit { Description = "µg/L" });

            if (!context.LabTestUnits.Any(ce => ce.Description == "Beats per minute"))
                labTestUnits.Add(new LabTestUnit { Description = "Beats per minute" });

            if (!context.LabTestUnits.Any(ce => ce.Description == "Breaths per minute"))
                labTestUnits.Add(new LabTestUnit { Description = "Breaths per minute" });

            if (!context.LabTestUnits.Any(ce => ce.Description == "Cavities"))
                labTestUnits.Add(new LabTestUnit { Description = "Cavities" });

            if (!context.LabTestUnits.Any(ce => ce.Description == "cells/mm 3"))
                labTestUnits.Add(new LabTestUnit { Description = "cells/mm 3" });

            if (!context.LabTestUnits.Any(ce => ce.Description == "g/dL"))
                labTestUnits.Add(new LabTestUnit { Description = "g/dL" });

            if (!context.LabTestUnits.Any(ce => ce.Description == "g/L"))
                labTestUnits.Add(new LabTestUnit { Description = "g/L" });

            if (!context.LabTestUnits.Any(ce => ce.Description == "IU/L"))
                labTestUnits.Add(new LabTestUnit { Description = "IU/L" });

            if (!context.LabTestUnits.Any(ce => ce.Description == "kg/m 2"))
                labTestUnits.Add(new LabTestUnit { Description = "kg/m 2" });

            if (!context.LabTestUnits.Any(ce => ce.Description == "mEq/L"))
                labTestUnits.Add(new LabTestUnit { Description = "mEq/L" });

            if (!context.LabTestUnits.Any(ce => ce.Description == "mg/24 hr"))
                labTestUnits.Add(new LabTestUnit { Description = "mg/24 hr" });

            if (!context.LabTestUnits.Any(ce => ce.Description == "mg/dL"))
                labTestUnits.Add(new LabTestUnit { Description = "mg/dL" });

            if (!context.LabTestUnits.Any(ce => ce.Description == "min"))
                labTestUnits.Add(new LabTestUnit { Description = "min" });

            if (!context.LabTestUnits.Any(ce => ce.Description == "mL/min"))
                labTestUnits.Add(new LabTestUnit { Description = "mL/min" });

            if (!context.LabTestUnits.Any(ce => ce.Description == "mm Hg"))
                labTestUnits.Add(new LabTestUnit { Description = "mm Hg" });

            if (!context.LabTestUnits.Any(ce => ce.Description == "mm/h"))
                labTestUnits.Add(new LabTestUnit { Description = "mm/h" });

            if (!context.LabTestUnits.Any(ce => ce.Description == "mmol/kg"))
                labTestUnits.Add(new LabTestUnit { Description = "mmol/kg" });

            if (!context.LabTestUnits.Any(ce => ce.Description == "mmol/L"))
                labTestUnits.Add(new LabTestUnit { Description = "mmol/L" });

            if (!context.LabTestUnits.Any(ce => ce.Description == "mOsm/kg"))
                labTestUnits.Add(new LabTestUnit { Description = "mOsm/kg" });

            if (!context.LabTestUnits.Any(ce => ce.Description == "ms"))
                labTestUnits.Add(new LabTestUnit { Description = "ms" });

            if (!context.LabTestUnits.Any(ce => ce.Description == "ng/dL"))
                labTestUnits.Add(new LabTestUnit { Description = "ng/dL" });

            if (!context.LabTestUnits.Any(ce => ce.Description == "ng/L"))
                labTestUnits.Add(new LabTestUnit { Description = "ng/L" });

            if (!context.LabTestUnits.Any(ce => ce.Description == "ng/mL"))
                labTestUnits.Add(new LabTestUnit { Description = "ng/mL" });

            if (!context.LabTestUnits.Any(ce => ce.Description == "ng/mL/hr"))
                labTestUnits.Add(new LabTestUnit { Description = "ng/mL/hr" });

            if (!context.LabTestUnits.Any(ce => ce.Description == "nmol/L"))
                labTestUnits.Add(new LabTestUnit { Description = "nmol/L" });

            if (!context.LabTestUnits.Any(ce => ce.Description == "pg/mL"))
                labTestUnits.Add(new LabTestUnit { Description = "pg/mL" });

            if (!context.LabTestUnits.Any(ce => ce.Description == "pH"))
                labTestUnits.Add(new LabTestUnit { Description = "pH" });

            if (!context.LabTestUnits.Any(ce => ce.Description == "pmol/L"))
                labTestUnits.Add(new LabTestUnit { Description = "pmol/L" });

            if (!context.LabTestUnits.Any(ce => ce.Description == "sec"))
                labTestUnits.Add(new LabTestUnit { Description = "sec" });

            if (!context.LabTestUnits.Any(ce => ce.Description == "X 10 3 /mm 3"))
                labTestUnits.Add(new LabTestUnit { Description = "X 10 3 /mm 3" });

            if (!context.LabTestUnits.Any(ce => ce.Description == "X 10 6 /mm 3"))
                labTestUnits.Add(new LabTestUnit { Description = "X 10 6 /mm 3" });

            if (!context.LabTestUnits.Any(ce => ce.Description == "X 10 9 /L"))
                labTestUnits.Add(new LabTestUnit { Description = "X 10 9 /L" });

            if (!context.LabTestUnits.Any(ce => ce.Description == "μg/dL"))
                labTestUnits.Add(new LabTestUnit { Description = "μg/dL" });

            if (!context.LabTestUnits.Any(ce => ce.Description == "μg/L"))
                labTestUnits.Add(new LabTestUnit { Description = "μg/L" });

            if (!context.LabTestUnits.Any(ce => ce.Description == "μmol/L"))
                labTestUnits.Add(new LabTestUnit { Description = "μmol/L" });

            if (!context.LabTestUnits.Any(ce => ce.Description == "μU/L"))
                labTestUnits.Add(new LabTestUnit { Description = "μU/L" });

            if (!context.LabTestUnits.Any(ce => ce.Description == "μU/mL"))
                labTestUnits.Add(new LabTestUnit { Description = "μU/mL" });

            return labTestUnits;
        }

        private IEnumerable<FieldType> PrepareFieldTypes(PVIMSDbContext context)
        {
            List<FieldType> fieldTypes = new List<FieldType>();

            if (!context.FieldTypes.Any(ce => ce.Description == "Listbox"))
                fieldTypes.Add(new FieldType { Description = "Listbox" });

            if (!context.FieldTypes.Any(ce => ce.Description == "DropDownList"))
                fieldTypes.Add(new FieldType { Description = "DropDownList" });

            if (!context.FieldTypes.Any(ce => ce.Description == "AlphaNumericTextbox"))
                fieldTypes.Add(new FieldType { Description = "AlphaNumericTextbox" });

            if (!context.FieldTypes.Any(ce => ce.Description == "NumericTextbox"))
                fieldTypes.Add(new FieldType { Description = "NumericTextbox" });

            if (!context.FieldTypes.Any(ce => ce.Description == "YesNo"))
                fieldTypes.Add(new FieldType { Description = "YesNo" });

            if (!context.FieldTypes.Any(ce => ce.Description == "Date"))
                fieldTypes.Add(new FieldType { Description = "Date" });

            if (!context.FieldTypes.Any(ce => ce.Description == "Table"))
                fieldTypes.Add(new FieldType { Description = "Table" });

            if (!context.FieldTypes.Any(ce => ce.Description == "System"))
                fieldTypes.Add(new FieldType { Description = "System" });

            return fieldTypes;
        }

        private IEnumerable<MetaTableType> PrepareMetaTableTypes(PVIMSDbContext context)
        {
            List<MetaTableType> metaTableTypes = new List<MetaTableType>();

            if (!context.MetaTableTypes.Any(ce => ce.Description == "Core"))
                metaTableTypes.Add(new MetaTableType { MetaTableTypeGuid = new Guid("7C1FE90D-2082-464E-AEFD-337E81088312"), Description = "Core" });

            if (!context.MetaTableTypes.Any(ce => ce.Description == "CoreChild"))
                metaTableTypes.Add(new MetaTableType { MetaTableTypeGuid = new Guid("94040819-D706-40BF-B3D1-66A0655A0BEF"), Description = "CoreChild" });

            if (!context.MetaTableTypes.Any(ce => ce.Description == "Child"))
                metaTableTypes.Add(new MetaTableType { MetaTableTypeGuid = new Guid("E4E98018-D67B-417E-973D-E25E36FF23DB"), Description = "Child" });

            if (!context.MetaTableTypes.Any(ce => ce.Description == "History"))
                metaTableTypes.Add(new MetaTableType { MetaTableTypeGuid = new Guid("F80D75CF-AAC7-48CE-A6FF-B8188F4B284C"), Description = "History" });

            if (!context.MetaTableTypes.Any(ce => ce.Description == "Lookup"))
                metaTableTypes.Add(new MetaTableType { MetaTableTypeGuid = new Guid("68FDA9E5-911A-48DD-895B-8DFD038BE41D"), Description = "Lookup" });

            return metaTableTypes;
        }

        private IEnumerable<MetaColumnType> PrepareMetaColumnTypes(PVIMSDbContext context)
        {
            List<MetaColumnType> metaColumnTypes = new List<MetaColumnType>();

            if (!context.MetaColumnTypes.Any(ce => ce.Description == "bigint"))
                metaColumnTypes.Add(new MetaColumnType { MetaColumnTypeGuid = new Guid("6E0741BE-C983-4144-A580-743739E0F058"), Description = "bigint" });

            if (!context.MetaColumnTypes.Any(ce => ce.Description == "binary"))
                metaColumnTypes.Add(new MetaColumnType { MetaColumnTypeGuid = new Guid("F7A38360-A0B1-44F7-BB62-B6F452768201"), Description = "binary" });

            if (!context.MetaColumnTypes.Any(ce => ce.Description == "bit"))
                metaColumnTypes.Add(new MetaColumnType { MetaColumnTypeGuid = new Guid("95C0CB7D-59AD-4A17-A63D-D87DE6C057F4"), Description = "bit" });

            if (!context.MetaColumnTypes.Any(ce => ce.Description == "char"))
                metaColumnTypes.Add(new MetaColumnType { MetaColumnTypeGuid = new Guid("D4A35D44-2A62-46BA-90B7-E0C1D3D92D06"), Description = "char" });

            if (!context.MetaColumnTypes.Any(ce => ce.Description == "date"))
                metaColumnTypes.Add(new MetaColumnType { MetaColumnTypeGuid = new Guid("0FCDDAF2-BB13-4969-8662-EAFA364F60F0"), Description = "date" });

            if (!context.MetaColumnTypes.Any(ce => ce.Description == "datetime"))
                metaColumnTypes.Add(new MetaColumnType { MetaColumnTypeGuid = new Guid("2E20E27D-5BCC-4B30-B72E-5187239D69EC"), Description = "datetime" });

            if (!context.MetaColumnTypes.Any(ce => ce.Description == "decimal"))
                metaColumnTypes.Add(new MetaColumnType { MetaColumnTypeGuid = new Guid("C9D0FDE5-6905-4640-96B0-F241EE4AD9C9"), Description = "decimal" });

            if (!context.MetaColumnTypes.Any(ce => ce.Description == "image"))
                metaColumnTypes.Add(new MetaColumnType { MetaColumnTypeGuid = new Guid("0D7F973E-E20A-4D52-9312-096C89CCDCCC"), Description = "image" });

            if (!context.MetaColumnTypes.Any(ce => ce.Description == "int"))
                metaColumnTypes.Add(new MetaColumnType { MetaColumnTypeGuid = new Guid("79BF57DA-FFB4-42B3-B062-29FF0D041F9F"), Description = "int" });

            if (!context.MetaColumnTypes.Any(ce => ce.Description == "nchar"))
                metaColumnTypes.Add(new MetaColumnType { MetaColumnTypeGuid = new Guid("C870A7A6-6D44-4034-88A4-D29F6E175B0E"), Description = "nchar" });

            if (!context.MetaColumnTypes.Any(ce => ce.Description == "nvarchar"))
                metaColumnTypes.Add(new MetaColumnType { MetaColumnTypeGuid = new Guid("8DA14F19-4BF6-41BE-A5D9-EDC112322874"), Description = "nvarchar" });

            if (!context.MetaColumnTypes.Any(ce => ce.Description == "smallint"))
                metaColumnTypes.Add(new MetaColumnType { MetaColumnTypeGuid = new Guid("88353A5F-48BF-4DE5-9549-F66E29CC6C5A"), Description = "smallint" });

            if (!context.MetaColumnTypes.Any(ce => ce.Description == "time"))
                metaColumnTypes.Add(new MetaColumnType { MetaColumnTypeGuid = new Guid("6C556210-EAAF-4044-8C86-59AA468A113D"), Description = "time" });

            if (!context.MetaColumnTypes.Any(ce => ce.Description == "tinyint"))
                metaColumnTypes.Add(new MetaColumnType { MetaColumnTypeGuid = new Guid("C2161983-0FBB-452F-94BD-E51DCD88A2D6"), Description = "tinyint" });

            if (!context.MetaColumnTypes.Any(ce => ce.Description == "uniqueidentifier"))
                metaColumnTypes.Add(new MetaColumnType { MetaColumnTypeGuid = new Guid("D8DF321C-4FA5-4BCD-AF16-9049DB860F46"), Description = "uniqueidentifier" });

            if (!context.MetaColumnTypes.Any(ce => ce.Description == "varbinary"))
                metaColumnTypes.Add(new MetaColumnType { MetaColumnTypeGuid = new Guid("46055898-959D-4264-A6BD-0015E2FC352A"), Description = "varbinary" });

            if (!context.MetaColumnTypes.Any(ce => ce.Description == "varchar"))
                metaColumnTypes.Add(new MetaColumnType { MetaColumnTypeGuid = new Guid("289C8787-9103-44ED-B101-99C7BF2270DB"), Description = "varchar" });

            return metaColumnTypes;
        }

        private IEnumerable<MetaWidgetType> PrepareMetaWidgetTypes(PVIMSDbContext context)
        {
            List<MetaWidgetType> metaWidgetTypes = new List<MetaWidgetType>();

            if (!context.MetaWidgetTypes.Any(ce => ce.Description == "General"))
                metaWidgetTypes.Add(new MetaWidgetType { MetaWidgetTypeGuid = new Guid("806DF7BC-404C-4DA5-A726-880E8BB0FEAA"), Description = "General" });

            if (!context.MetaWidgetTypes.Any(ce => ce.Description == "SubItems"))
                metaWidgetTypes.Add(new MetaWidgetType { MetaWidgetTypeGuid = new Guid("58DD7339-B9E0-4CBB-A47B-B2C027D6DF86"), Description = "SubItems" });

            if (!context.MetaWidgetTypes.Any(ce => ce.Description == "ItemList"))
                metaWidgetTypes.Add(new MetaWidgetType { MetaWidgetTypeGuid = new Guid("7179F12D-668E-4A5D-A676-D9C1FEBDC6A4"), Description = "ItemList" });

            return metaWidgetTypes;
        }

        private IEnumerable<Config> PrepareConfigs(PVIMSDbContext context)
        {
            List<Config> configs = new List<Config>();

            if (!context.Configs.Any(ce => ce.ConfigType == ConfigType.E2BVersion))
            {
                var e2bConfig = new Config { ConfigType = ConfigType.E2BVersion, ConfigValue = "E2B(R2) ICH Report" };
                e2bConfig.AuditStamp(context.Users.Single(u => u.UserName == "Admin"));
                configs.Add(e2bConfig);
            }

            if (!context.Configs.Any(ce => ce.ConfigType == ConfigType.WebServiceSubscriberList))
            {
                var subscriberConfig = new Config { ConfigType = ConfigType.WebServiceSubscriberList, ConfigValue = "NOT SPECIFIED" };
                subscriberConfig.AuditStamp(context.Users.Single(u => u.UserName == "Admin"));
                configs.Add(subscriberConfig);
            }

            if (!context.Configs.Any(ce => ce.ConfigType == ConfigType.AssessmentScale))
            {
                var assessmentConfig = new Config { ConfigType = ConfigType.AssessmentScale, ConfigValue = "Both Scales" };
                assessmentConfig.AuditStamp(context.Users.Single(u => u.UserName == "Admin"));
                configs.Add(assessmentConfig);
            }

            if (!context.Configs.Any(ce => ce.ConfigType == ConfigType.MedDRAVersion))
            {
                var meddraConfig = new Config { ConfigType = ConfigType.MedDRAVersion, ConfigValue = "23.0" };
                meddraConfig.AuditStamp(context.Users.Single(u => u.UserName == "Admin"));
                configs.Add(meddraConfig);
            }

            if (!context.Configs.Any(ce => ce.ConfigType == ConfigType.ReportInstanceNewAlertCount))
            {
                var instanceConfig = new Config { ConfigType = ConfigType.ReportInstanceNewAlertCount, ConfigValue = "0" };
                instanceConfig.AuditStamp(context.Users.Single(u => u.UserName == "Admin"));
                configs.Add(instanceConfig);
            }

            if (!context.Configs.Any(ce => ce.ConfigType == ConfigType.MedicationOnsetCheckPeriodWeeks))
            {
                var onsetConfig = new Config { ConfigType = ConfigType.MedicationOnsetCheckPeriodWeeks, ConfigValue = "5" };
                onsetConfig.AuditStamp(context.Users.Single(u => u.UserName == "Admin"));
                configs.Add(onsetConfig);
            }

            if (!context.Configs.Any(ce => ce.ConfigType == ConfigType.MetaDataLastUpdated))
            {
                var metaConfig = new Config { ConfigType = ConfigType.MetaDataLastUpdated, ConfigValue = DateTime.Now.ToString("yyyy-MM-dd HH:mm") };
                metaConfig.AuditStamp(context.Users.Single(u => u.UserName == "Admin"));
                configs.Add(metaConfig);
            }

            if (!context.Configs.Any(ce => ce.ConfigType == ConfigType.PharmadexLink))
            {
                var pharmadexConfig = new Config { ConfigType = ConfigType.PharmadexLink, ConfigValue = "NOT SPECIFIED" };
                pharmadexConfig.AuditStamp(context.Users.Single(u => u.UserName == "Admin"));
                configs.Add(pharmadexConfig);
            }

            return configs;
        }

        private IEnumerable<ContextType> PrepareContextTypes(PVIMSDbContext context)
        {
            List<ContextType> contextTypes = new List<ContextType>();

            if (!context.ContextTypes.Any(ce => ce.Description == "Encounter"))
                contextTypes.Add(new ContextType { Description = "Encounter" });

            if (!context.ContextTypes.Any(ce => ce.Description == "Patient"))
                contextTypes.Add(new ContextType { Description = "Patient" });

            if (!context.ContextTypes.Any(ce => ce.Description == "Pregnancy"))
                contextTypes.Add(new ContextType { Description = "Pregnancy" });

            if (!context.ContextTypes.Any(ce => ce.Description == "Global"))
                contextTypes.Add(new ContextType { Description = "Global" });

            if (!context.ContextTypes.Any(ce => ce.Description == "PatientClinicalEvent"))
                contextTypes.Add(new ContextType { Description = "PatientClinicalEvent" });

            if (!context.ContextTypes.Any(ce => ce.Description == "DatasetInstance"))
                contextTypes.Add(new ContextType { Description = "DatasetInstance" });

            return contextTypes;
        }

        private AsyncRetryPolicy CreatePolicy(ILogger<PVIMSContextSeed> logger, string prefix, int retries = 3)
        {
            return Policy.Handle<SqlException>().
                WaitAndRetryAsync(
                    retryCount: retries,
                    sleepDurationProvider: retry => TimeSpan.FromSeconds(5),
                    onRetry: (exception, timeSpan, retry, ctx) =>
                    {
                        logger.LogWarning(exception, "[{prefix}] Exception {ExceptionType} with message {Message} detected on attempt {retry} of {retries}", prefix, exception.GetType().Name, exception.Message, retry, retries);
                    }
                );
        }
    }
}
