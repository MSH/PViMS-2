﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PViMS.Infrastructure.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AttachmentType",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Key = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttachmentType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CareEvent",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CareEvent", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Condition",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Chronic = table.Column<bool>(type: "bit", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Condition", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ContextTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContextTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CustomAttributeConfiguration",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExtendableTypeName = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CustomAttributeType = table.Column<int>(type: "int", nullable: false),
                    Category = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AttributeKey = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    AttributeDetail = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    IsRequired = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    StringMaxLength = table.Column<int>(type: "int", nullable: true),
                    NumericMinValue = table.Column<int>(type: "int", nullable: true),
                    NumericMaxValue = table.Column<int>(type: "int", nullable: true),
                    FutureDateOnly = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    PastDateOnly = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    IsSearchable = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomAttributeConfiguration", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DatasetElementType",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DatasetElementType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EncounterType",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Help = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    Chronic = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EncounterType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FacilityType",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FacilityType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FieldType",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FieldType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Holiday",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HolidayDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Holiday", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LabResult",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LabResult", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LabTest",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LabTest", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LabTestUnit",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LabTestUnit", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Language",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Language", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MedicationForm",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicationForm", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MetaColumnType",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    metacolumntype_guid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MetaColumnType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MetaForm",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    metaform_guid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FormName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MetaDefinition = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsSystem = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    ActionName = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MetaForm", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MetaPage",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    metapage_guid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PageName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PageDefinition = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    MetaDefinition = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Breadcrumb = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    IsSystem = table.Column<bool>(type: "bit", nullable: false),
                    IsVisible = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MetaPage", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MetaReport",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    metareport_guid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReportName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ReportDefinition = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    MetaDefinition = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Breadcrumb = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    SQLDefinition = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsSystem = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    ReportStatus = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MetaReport", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MetaTableType",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    metatabletype_guid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MetaTableType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MetaWidgetType",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    metawidgettype_guid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MetaWidgetType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OrgUnitTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ParentId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrgUnitTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrgUnitTypes_OrgUnitTypes_ParentId",
                        column: x => x.ParentId,
                        principalTable: "OrgUnitTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Outcome",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Outcome", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PatientStatus",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientStatus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PostDeployment",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ScriptGuid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ScriptFileName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ScriptDescription = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    RunDate = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: true),
                    StatusCode = table.Column<int>(type: "int", nullable: true),
                    StatusMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RunRank = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostDeployment", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Priority",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Priority", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RiskFactor",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FactorName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Criteria = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Display = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    IsSystem = table.Column<bool>(type: "bit", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RiskFactor", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Role",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Key = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Role", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SelectionDataItem",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AttributeKey = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    SelectionKey = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SelectionDataItem", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TerminologyIcd10",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TerminologyIcd10", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TerminologyMedDra",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MedDraTerm = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MedDraCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    MedDraTermType = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: false),
                    Parent_Id = table.Column<int>(type: "int", nullable: true),
                    MedDraVersion = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: true),
                    Common = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TerminologyMedDra", x => x.Id);
                    table.ForeignKey(
                        name: "FK_dbo.TerminologyMedDra_dbo.TerminologyMedDra_Parent_Id",
                        column: x => x.Parent_Id,
                        principalTable: "TerminologyMedDra",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TreatmentOutcome",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TreatmentOutcome", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    LockoutEndDateUtc = table.Column<DateTime>(type: "datetime", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    CurrentContext = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    EulaAcceptanceDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    AllowDatasetDownload = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    IdentityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserType = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WorkFlow",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    WorkFlowGuid = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkFlow", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CohortGroup",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CohortName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CohortCode = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    LastPatientNo = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    StartDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    FinishDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    MinEnrolment = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    MaxEnrolment = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    Condition_Id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CohortGroup", x => x.Id);
                    table.ForeignKey(
                        name: "FK_dbo.CohortGroup_dbo.Condition_Condition_Id",
                        column: x => x.Condition_Id,
                        principalTable: "Condition",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Field",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Mandatory = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    MaxLength = table.Column<short>(type: "smallint", nullable: true),
                    RegEx = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Decimals = table.Column<short>(type: "smallint", nullable: true),
                    MaxSize = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    MinSize = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Calculation = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Image = table.Column<byte[]>(type: "image", nullable: true),
                    FileSize = table.Column<short>(type: "smallint", nullable: true),
                    FileExt = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Anonymise = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    FieldType_Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Field", x => x.Id);
                    table.ForeignKey(
                        name: "FK_dbo.Field_dbo.FieldType_FieldType_Id",
                        column: x => x.FieldType_Id,
                        principalTable: "FieldType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ConditionLabTest",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Condition_Id = table.Column<int>(type: "int", nullable: false),
                    LabTest_Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConditionLabTest", x => x.Id);
                    table.ForeignKey(
                        name: "FK_dbo.ConditionLabTest_dbo.Condition_Condition_Id",
                        column: x => x.Condition_Id,
                        principalTable: "Condition",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_dbo.ConditionLabTest_dbo.LabTest_LabTest_Id",
                        column: x => x.LabTest_Id,
                        principalTable: "LabTest",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Concept",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ConceptName = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    MedicationForm_Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Concept", x => x.Id);
                    table.ForeignKey(
                        name: "FK_dbo.Concept_dbo.MedicationForm_MedicationForm_Id",
                        column: x => x.MedicationForm_Id,
                        principalTable: "MedicationForm",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MetaTable",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    metatable_guid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TableName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FriendlyName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    FriendlyDescription = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    TableType_Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MetaTable", x => x.Id);
                    table.ForeignKey(
                        name: "FK_dbo.MetaTable_dbo.MetaTableType_TableType_Id",
                        column: x => x.TableType_Id,
                        principalTable: "MetaTableType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MetaWidget",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    metawidget_guid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WidgetName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    WidgetDefinition = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WidgetType_Id = table.Column<int>(type: "int", nullable: false),
                    MetaPage_Id = table.Column<int>(type: "int", nullable: false),
                    WidgetLocation = table.Column<int>(type: "int", nullable: false),
                    WidgetStatus = table.Column<int>(type: "int", nullable: false),
                    Icon = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MetaWidget", x => x.Id);
                    table.ForeignKey(
                        name: "FK_dbo.MetaWidget_dbo.MetaPage_MetaPage_Id",
                        column: x => x.MetaPage_Id,
                        principalTable: "MetaPage",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_dbo.MetaWidget_dbo.MetaWidgetType_WidgetType_Id",
                        column: x => x.WidgetType_Id,
                        principalTable: "MetaWidgetType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OrgUnit",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Parent_Id = table.Column<int>(type: "int", nullable: true),
                    OrgUnitType_Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrgUnit", x => x.Id);
                    table.ForeignKey(
                        name: "FK_dbo.OrgUnit_dbo.OrgUnit_Parent_Id",
                        column: x => x.Parent_Id,
                        principalTable: "OrgUnit",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_dbo.OrgUnit_dbo.OrgUnitType_OrgUnitType_Id",
                        column: x => x.OrgUnitType_Id,
                        principalTable: "OrgUnitTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RiskFactorOption",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OptionName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Criteria = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Display = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    RiskFactor_Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RiskFactorOption", x => x.Id);
                    table.ForeignKey(
                        name: "FK_dbo.RiskFactorOption_dbo.RiskFactor_RiskFactor_Id",
                        column: x => x.RiskFactor_Id,
                        principalTable: "RiskFactor",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ConditionMedDra",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Condition_Id = table.Column<int>(type: "int", nullable: false),
                    TerminologyMedDra_Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConditionMedDra", x => x.Id);
                    table.ForeignKey(
                        name: "FK_dbo.ConditionMedDra_dbo.Condition_Condition_Id",
                        column: x => x.Condition_Id,
                        principalTable: "Condition",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_dbo.ConditionMedDra_dbo.TerminologyMedDra_TerminologyMedDra_Id",
                        column: x => x.TerminologyMedDra_Id,
                        principalTable: "TerminologyMedDra",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MedDRAScale",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GradingScale_Id = table.Column<int>(type: "int", nullable: false),
                    TerminologyMedDra_Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedDRAScale", x => x.Id);
                    table.ForeignKey(
                        name: "FK_dbo.MedDRAScale_dbo.SelectionDataItem_GradingScale_Id",
                        column: x => x.GradingScale_Id,
                        principalTable: "SelectionDataItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_dbo.MedDRAScale_dbo.TerminologyMedDra_TerminologyMedDra_Id",
                        column: x => x.TerminologyMedDra_Id,
                        principalTable: "TerminologyMedDra",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AuditLog",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ActionDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    Details = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    User_Id = table.Column<int>(type: "int", nullable: false),
                    Log = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AuditType = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLog", x => x.Id);
                    table.ForeignKey(
                        name: "FK_dbo.AuditLog_dbo.User_User_Id",
                        column: x => x.User_Id,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Config",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ConfigValue = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ConfigType = table.Column<int>(type: "int", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime", nullable: true),
                    CreatedBy_Id = table.Column<int>(type: "int", nullable: false),
                    UpdatedBy_Id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Config", x => x.Id);
                    table.ForeignKey(
                        name: "FK_dbo.Config_dbo.User_CreatedBy_Id",
                        column: x => x.CreatedBy_Id,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_dbo.Config_dbo.User_UpdatedBy_Id",
                        column: x => x.UpdatedBy_Id,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DatasetXml",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Created = table.Column<DateTime>(type: "datetime", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime", nullable: true),
                    CreatedBy_Id = table.Column<int>(type: "int", nullable: false),
                    UpdatedBy_Id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DatasetXml", x => x.Id);
                    table.ForeignKey(
                        name: "FK_dbo.DatasetXml_dbo.User_CreatedBy_Id",
                        column: x => x.CreatedBy_Id,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_dbo.DatasetXml_dbo.User_UpdatedBy_Id",
                        column: x => x.UpdatedBy_Id,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Patient",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DateOfBirth = table.Column<DateTime>(type: "date", nullable: true),
                    FirstName = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Surname = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PatientGuid = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "newid()"),
                    MiddleName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Archived = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    ArchivedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    ArchivedReason = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    AuditUser_Id = table.Column<int>(type: "int", nullable: true),
                    CustomAttributesXmlSerialised = table.Column<string>(type: "xml", nullable: true),
                    Created = table.Column<DateTime>(type: "datetime", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime", nullable: true),
                    CreatedBy_Id = table.Column<int>(type: "int", nullable: false),
                    UpdatedBy_Id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Patient", x => x.Id);
                    table.ForeignKey(
                        name: "FK_dbo.Patient_dbo.User_AuditUser_Id",
                        column: x => x.AuditUser_Id,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_dbo.Patient_dbo.User_CreatedBy_Id",
                        column: x => x.CreatedBy_Id,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_dbo.Patient_dbo.User_UpdatedBy_Id",
                        column: x => x.UpdatedBy_Id,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RefreshToken",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Token = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Expires = table.Column<DateTime>(type: "datetime", nullable: false),
                    RemoteIpAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    User_Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshToken", x => x.Id);
                    table.ForeignKey(
                        name: "FK_dbo.RefreshToken_dbo.User_User_Id",
                        column: x => x.User_Id,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SiteContactDetail",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ContactType = table.Column<int>(type: "int", nullable: false),
                    ContactFirstName = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    ContactSurname = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    StreetAddress = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    City = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    State = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PostCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    ContactNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ContactEmail = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CountryCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    OrganisationName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Created = table.Column<DateTime>(type: "datetime", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime", nullable: true),
                    CreatedBy_Id = table.Column<int>(type: "int", nullable: false),
                    UpdatedBy_Id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SiteContactDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_dbo.SiteContactDetail_dbo.User_CreatedBy_Id",
                        column: x => x.CreatedBy_Id,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_dbo.SiteContactDetail_dbo.User_UpdatedBy_Id",
                        column: x => x.UpdatedBy_Id,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SystemLog",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Sender = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EventType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExceptionCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExceptionMessage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExceptionStackTrace = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InnerExceptionMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InnerExceptionStackTrace = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RemoteIpAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Created = table.Column<DateTime>(type: "datetime", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime", nullable: true),
                    CreatedBy_Id = table.Column<int>(type: "int", nullable: false),
                    UpdatedBy_Id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemLog", x => x.Id);
                    table.ForeignKey(
                        name: "FK_dbo.SystemLog_dbo.User_CreatedBy_Id",
                        column: x => x.CreatedBy_Id,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_dbo.SystemLog_dbo.User_UpdatedBy_Id",
                        column: x => x.UpdatedBy_Id,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserRole",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Role_Id = table.Column<int>(type: "int", nullable: false),
                    User_Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRole", x => x.Id);
                    table.ForeignKey(
                        name: "FK_dbo.UserRole_dbo.Role_Role_Id",
                        column: x => x.Role_Id,
                        principalTable: "Role",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_dbo.UserRole_dbo.User_User_Id",
                        column: x => x.User_Id,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Activity",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QualifiedName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    WorkFlow_Id = table.Column<int>(type: "int", nullable: false),
                    ActivityType = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Activity", x => x.Id);
                    table.ForeignKey(
                        name: "FK_dbo.Activity_dbo.WorkFlow_WorkFlow_Id",
                        column: x => x.WorkFlow_Id,
                        principalTable: "WorkFlow",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReportInstance",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReportInstanceGuid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Finished = table.Column<DateTime>(type: "datetime", nullable: true),
                    WorkFlow_Id = table.Column<int>(type: "int", nullable: false),
                    ContextGuid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Identifier = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PatientIdentifier = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TerminologyMedDra_Id = table.Column<int>(type: "int", nullable: true),
                    SourceIdentifier = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime", nullable: true),
                    CreatedBy_Id = table.Column<int>(type: "int", nullable: false),
                    UpdatedBy_Id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportInstance", x => x.Id);
                    table.ForeignKey(
                        name: "FK_dbo.ReportInstance_dbo.TerminologyMedDra_TerminologyMedDra_Id",
                        column: x => x.TerminologyMedDra_Id,
                        principalTable: "TerminologyMedDra",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_dbo.ReportInstance_dbo.User_CreatedBy_Id",
                        column: x => x.CreatedBy_Id,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_dbo.ReportInstance_dbo.User_UpdatedBy_Id",
                        column: x => x.UpdatedBy_Id,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_dbo.ReportInstance_dbo.WorkFlow_WorkFlow_Id",
                        column: x => x.WorkFlow_Id,
                        principalTable: "WorkFlow",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DatasetElement",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ElementName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Field_Id = table.Column<int>(type: "int", nullable: false),
                    DatasetElementType_Id = table.Column<int>(type: "int", nullable: false),
                    OID = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    DefaultValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    System = table.Column<bool>(type: "bit", nullable: false),
                    UID = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    DatasetElementGuid = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "newid()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DatasetElement", x => x.Id);
                    table.ForeignKey(
                        name: "FK_dbo.DatasetElement_dbo.DatasetElementType_DatasetElementType_Id",
                        column: x => x.DatasetElementType_Id,
                        principalTable: "DatasetElementType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_dbo.DatasetElement_dbo.Field_Field_Id",
                        column: x => x.Field_Id,
                        principalTable: "Field",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FieldValue",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Value = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Default = table.Column<bool>(type: "bit", nullable: false),
                    Other = table.Column<bool>(type: "bit", nullable: false),
                    Unknown = table.Column<bool>(type: "bit", nullable: false),
                    Field_Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FieldValue", x => x.Id);
                    table.ForeignKey(
                        name: "FK_dbo.FieldValue_dbo.Field_Field_Id",
                        column: x => x.Field_Id,
                        principalTable: "Field",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ConceptIngredient",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ingredient = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Strength = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    Concept_Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConceptIngredient", x => x.Id);
                    table.ForeignKey(
                        name: "FK_dbo.ConceptIngredient_dbo.Concept_Concept_Id",
                        column: x => x.Concept_Id,
                        principalTable: "Concept",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Product",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Manufacturer = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    Concept_Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Product", x => x.Id);
                    table.ForeignKey(
                        name: "FK_dbo.Product_dbo.Concept_Concept_Id",
                        column: x => x.Concept_Id,
                        principalTable: "Concept",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MetaColumn",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    metacolumn_guid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ColumnName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsIdentity = table.Column<bool>(type: "bit", nullable: false),
                    IsNullable = table.Column<bool>(type: "bit", nullable: false),
                    ColumnType_Id = table.Column<int>(type: "int", nullable: false),
                    Table_Id = table.Column<int>(type: "int", nullable: false),
                    Range = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MetaColumn", x => x.Id);
                    table.ForeignKey(
                        name: "FK_dbo.MetaColumn_dbo.MetaColumnType_ColumnType_Id",
                        column: x => x.ColumnType_Id,
                        principalTable: "MetaColumnType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_dbo.MetaColumn_dbo.MetaTable_Table_Id",
                        column: x => x.Table_Id,
                        principalTable: "MetaTable",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MetaDependency",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    metadependency_guid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ParentColumnName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ReferenceColumnName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ParentTable_Id = table.Column<int>(type: "int", nullable: false),
                    ReferenceTable_Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MetaDependency", x => x.Id);
                    table.ForeignKey(
                        name: "FK_dbo.MetaDependency_dbo.MetaTable_ParentTable_Id",
                        column: x => x.ParentTable_Id,
                        principalTable: "MetaTable",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_dbo.MetaDependency_dbo.MetaTable_ReferenceTable_Id",
                        column: x => x.ReferenceTable_Id,
                        principalTable: "MetaTable",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Facility",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FacilityCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    FacilityName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FacilityType_Id = table.Column<int>(type: "int", nullable: false),
                    TelNumber = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    MobileNumber = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    FaxNumber = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    OrgUnit_Id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Facility", x => x.Id);
                    table.ForeignKey(
                        name: "FK_dbo.Facility_dbo.FacilityType_FacilityType_Id",
                        column: x => x.FacilityType_Id,
                        principalTable: "FacilityType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_dbo.Facility_dbo.OrgUnit_OrgUnit_Id",
                        column: x => x.OrgUnit_Id,
                        principalTable: "OrgUnit",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MedDRAGrading",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Grade = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Scale_Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedDRAGrading", x => x.Id);
                    table.ForeignKey(
                        name: "FK_dbo.MedDRAGrading_dbo.MedDRAScale_Scale_Id",
                        column: x => x.Scale_Id,
                        principalTable: "MedDRAScale",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Appointment",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AppointmentDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    DNA = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    Cancelled = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CancellationReason = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    Patient_Id = table.Column<int>(type: "int", nullable: false),
                    Archived = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    ArchivedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    ArchivedReason = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    AuditUser_Id = table.Column<int>(type: "int", nullable: true),
                    Created = table.Column<DateTime>(type: "datetime", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime", nullable: true),
                    CreatedBy_Id = table.Column<int>(type: "int", nullable: false),
                    UpdatedBy_Id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Appointment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_dbo.Appointment_dbo.Patient_Patient_Id",
                        column: x => x.Patient_Id,
                        principalTable: "Patient",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_dbo.Appointment_dbo.User_AuditUser_Id",
                        column: x => x.AuditUser_Id,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_dbo.Appointment_dbo.User_CreatedBy_Id",
                        column: x => x.CreatedBy_Id,
                        principalTable: "User",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_dbo.Appointment_dbo.User_UpdatedBy_Id",
                        column: x => x.UpdatedBy_Id,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CohortGroupEnrolment",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EnroledDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    CohortGroup_Id = table.Column<int>(type: "int", nullable: false),
                    Patient_Id = table.Column<int>(type: "int", nullable: false),
                    DeenroledDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    Archived = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    ArchivedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    ArchivedReason = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    AuditUser_Id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CohortGroupEnrolment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_dbo.CohortGroupEnrolment_dbo.CohortGroup_CohortGroup_Id",
                        column: x => x.CohortGroup_Id,
                        principalTable: "CohortGroup",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_dbo.CohortGroupEnrolment_dbo.Patient_Patient_Id",
                        column: x => x.Patient_Id,
                        principalTable: "Patient",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_dbo.CohortGroupEnrolment_dbo.User_AuditUser_Id",
                        column: x => x.AuditUser_Id,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PatientCondition",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DateStart = table.Column<DateTime>(type: "datetime", nullable: false),
                    OutcomeDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    Comments = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Condition_Id = table.Column<int>(type: "int", nullable: true),
                    Patient_Id = table.Column<int>(type: "int", nullable: false),
                    PatientConditionGuid = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    TerminologyMedDra_Id = table.Column<int>(type: "int", nullable: true),
                    Archived = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    ArchivedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    ArchivedReason = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    AuditUser_Id = table.Column<int>(type: "int", nullable: true),
                    Outcome_Id = table.Column<int>(type: "int", nullable: true),
                    TreatmentOutcome_Id = table.Column<int>(type: "int", nullable: true),
                    ConditionSource = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CustomAttributesXmlSerialised = table.Column<string>(type: "xml", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientCondition", x => x.Id);
                    table.ForeignKey(
                        name: "FK_dbo.PatientCondition_dbo.Condition_Condition_Id",
                        column: x => x.Condition_Id,
                        principalTable: "Condition",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_dbo.PatientCondition_dbo.Outcome_Outcome_Id",
                        column: x => x.Outcome_Id,
                        principalTable: "Outcome",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_dbo.PatientCondition_dbo.Patient_Patient_Id",
                        column: x => x.Patient_Id,
                        principalTable: "Patient",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_dbo.PatientCondition_dbo.TerminologyMedDra_TerminologyMedDra_Id",
                        column: x => x.TerminologyMedDra_Id,
                        principalTable: "TerminologyMedDra",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_dbo.PatientCondition_dbo.TreatmentOutcome_TreatmentOutcome_Id",
                        column: x => x.TreatmentOutcome_Id,
                        principalTable: "TreatmentOutcome",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_dbo.PatientCondition_dbo.User_AuditUser_Id",
                        column: x => x.AuditUser_Id,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PatientLabTest",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TestDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    TestResult = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    LabTest_Id = table.Column<int>(type: "int", nullable: false),
                    Patient_Id = table.Column<int>(type: "int", nullable: false),
                    PatientLabTestGuid = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    TestUnit_Id = table.Column<int>(type: "int", nullable: true),
                    LabValue = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Archived = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    ArchivedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    ArchivedReason = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    AuditUser_Id = table.Column<int>(type: "int", nullable: true),
                    ReferenceLower = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    ReferenceUpper = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    LabTestSource = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CustomAttributesXmlSerialised = table.Column<string>(type: "xml", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientLabTest", x => x.Id);
                    table.ForeignKey(
                        name: "FK_dbo.PatientLabTest_dbo.LabTest_LabTest_Id",
                        column: x => x.LabTest_Id,
                        principalTable: "LabTest",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_dbo.PatientLabTest_dbo.LabTestUnit_TestUnit_Id",
                        column: x => x.TestUnit_Id,
                        principalTable: "LabTestUnit",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_dbo.PatientLabTest_dbo.Patient_Patient_Id",
                        column: x => x.Patient_Id,
                        principalTable: "Patient",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_dbo.PatientLabTest_dbo.User_AuditUser_Id",
                        column: x => x.AuditUser_Id,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PatientLanguage",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Preferred = table.Column<bool>(type: "bit", nullable: false),
                    Language_Id = table.Column<int>(type: "int", nullable: false),
                    Patient_Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientLanguage", x => x.Id);
                    table.ForeignKey(
                        name: "FK_dbo.PatientLanguage_dbo.Language_Language_Id",
                        column: x => x.Language_Id,
                        principalTable: "Language",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_dbo.PatientLanguage_dbo.Patient_Patient_Id",
                        column: x => x.Patient_Id,
                        principalTable: "Patient",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PatientStatusHistory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EffectiveDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    Comments = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Patient_Id = table.Column<int>(type: "int", nullable: false),
                    PatientStatus_Id = table.Column<int>(type: "int", nullable: false),
                    Archived = table.Column<bool>(type: "bit", nullable: false),
                    ArchivedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    ArchivedReason = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    AuditUser_Id = table.Column<int>(type: "int", nullable: true),
                    Created = table.Column<DateTime>(type: "datetime", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime", nullable: true),
                    CreatedBy_Id = table.Column<int>(type: "int", nullable: false),
                    UpdatedBy_Id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientStatusHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_dbo.PatientStatusHistory_dbo.Patient_Patient_Id",
                        column: x => x.Patient_Id,
                        principalTable: "Patient",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_dbo.PatientStatusHistory_dbo.PatientStatus_PatientStatus_Id",
                        column: x => x.PatientStatus_Id,
                        principalTable: "PatientStatus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_dbo.PatientStatusHistory_dbo.User_AuditUser_Id",
                        column: x => x.AuditUser_Id,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_dbo.PatientStatusHistory_dbo.User_CreatedBy_Id",
                        column: x => x.CreatedBy_Id,
                        principalTable: "User",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_dbo.PatientStatusHistory_dbo.User_UpdatedBy_Id",
                        column: x => x.UpdatedBy_Id,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Pregnancy",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StartDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    FinishDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    PreferredFeedingChoice = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    InitialGestation = table.Column<short>(type: "smallint", nullable: true),
                    ExpectedDeliveryDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    ActualDeliveryDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    Patient_Id = table.Column<int>(type: "int", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime", nullable: true),
                    CreatedBy_Id = table.Column<int>(type: "int", nullable: false),
                    UpdatedBy_Id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pregnancy", x => x.Id);
                    table.ForeignKey(
                        name: "FK_dbo.Pregnancy_dbo.Patient_Patient_Id",
                        column: x => x.Patient_Id,
                        principalTable: "Patient",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_dbo.Pregnancy_dbo.User_CreatedBy_Id",
                        column: x => x.CreatedBy_Id,
                        principalTable: "User",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_dbo.Pregnancy_dbo.User_UpdatedBy_Id",
                        column: x => x.UpdatedBy_Id,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ActivityExecutionStatus",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Activity_Id = table.Column<int>(type: "int", nullable: false),
                    FriendlyDescription = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Activity_Id1 = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivityExecutionStatus", x => x.Id);
                    table.ForeignKey(
                        name: "FK_dbo.ActivityExecutionStatus_dbo.Activity_Activity_Id",
                        column: x => x.Activity_Id,
                        principalTable: "Activity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReportInstanceMedication",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MedicationIdentifier = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NaranjoCausality = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    WhoCausality = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    ReportInstance_Id = table.Column<int>(type: "int", nullable: false),
                    ReportInstanceMedicationGuid = table.Column<Guid>(type: "uniqueidentifier", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportInstanceMedication", x => x.Id);
                    table.ForeignKey(
                        name: "FK_dbo.ReportInstanceMedication_dbo.ReportInstance_ReportInstance_Id",
                        column: x => x.ReportInstance_Id,
                        principalTable: "ReportInstance",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DatasetElementSub",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ElementName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FieldOrder = table.Column<short>(type: "smallint", nullable: false),
                    DatasetElement_Id = table.Column<int>(type: "int", nullable: false),
                    Field_Id = table.Column<int>(type: "int", nullable: false),
                    OID = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    DefaultValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    System = table.Column<bool>(type: "bit", nullable: false),
                    FriendlyName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Help = table.Column<string>(type: "nvarchar(350)", maxLength: 350, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DatasetElementSub", x => x.Id);
                    table.ForeignKey(
                        name: "FK_dbo.DatasetElementSub_dbo.DatasetElement_DatasetElement_Id",
                        column: x => x.DatasetElement_Id,
                        principalTable: "DatasetElement",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_dbo.DatasetElementSub_dbo.Field_Field_Id",
                        column: x => x.Field_Id,
                        principalTable: "Field",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ConditionMedication",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Condition_Id = table.Column<int>(type: "int", nullable: false),
                    Concept_Id = table.Column<int>(type: "int", nullable: false),
                    Product_Id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConditionMedication", x => x.Id);
                    table.ForeignKey(
                        name: "FK_dbo.ConditionMedication_dbo.Concept_Concept_Id",
                        column: x => x.Concept_Id,
                        principalTable: "Concept",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_dbo.ConditionMedication_dbo.Condition_Condition_Id",
                        column: x => x.Condition_Id,
                        principalTable: "Condition",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_dbo.ConditionMedication_dbo.Product_Product_Id",
                        column: x => x.Product_Id,
                        principalTable: "Product",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PatientMedication",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DateStart = table.Column<DateTime>(type: "datetime", nullable: false),
                    DateEnd = table.Column<DateTime>(type: "datetime", nullable: true),
                    Dose = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    DoseFrequency = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    DoseUnit = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Patient_Id = table.Column<int>(type: "int", nullable: false),
                    PatientMedicationGuid = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    Archived = table.Column<bool>(type: "bit", nullable: false),
                    ArchivedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    ArchivedReason = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    AuditUser_Id = table.Column<int>(type: "int", nullable: true),
                    MedicationSource = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Concept_Id = table.Column<int>(type: "int", nullable: false),
                    Product_Id = table.Column<int>(type: "int", nullable: true),
                    CustomAttributesXmlSerialised = table.Column<string>(type: "xml", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientMedication", x => x.Id);
                    table.ForeignKey(
                        name: "FK_dbo.PatientMedication_dbo.Concept_Concept_Id",
                        column: x => x.Concept_Id,
                        principalTable: "Concept",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_dbo.PatientMedication_dbo.Patient_Patient_Id",
                        column: x => x.Patient_Id,
                        principalTable: "Patient",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_dbo.PatientMedication_dbo.Product_Product_Id",
                        column: x => x.Product_Id,
                        principalTable: "Product",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_dbo.PatientMedication_dbo.User_AuditUser_Id",
                        column: x => x.AuditUser_Id,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PatientFacility",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EnrolledDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    Facility_Id = table.Column<int>(type: "int", nullable: false),
                    Patient_Id = table.Column<int>(type: "int", nullable: false),
                    Archived = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    ArchivedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    ArchivedReason = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    AuditUser_Id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientFacility", x => x.Id);
                    table.ForeignKey(
                        name: "FK_dbo.PatientFacility_dbo.Facility_Facility_Id",
                        column: x => x.Facility_Id,
                        principalTable: "Facility",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_dbo.PatientFacility_dbo.Patient_Patient_Id",
                        column: x => x.Patient_Id,
                        principalTable: "Patient",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_dbo.PatientFacility_dbo.User_AuditUser_Id",
                        column: x => x.AuditUser_Id,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserFacility",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Facility_Id = table.Column<int>(type: "int", nullable: false),
                    User_Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserFacility", x => x.Id);
                    table.ForeignKey(
                        name: "FK_dbo.UserFacility_dbo.Facility_Facility_Id",
                        column: x => x.Facility_Id,
                        principalTable: "Facility",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_dbo.UserFacility_dbo.User_User_Id",
                        column: x => x.User_Id,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Encounter",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EncounterDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EncounterGuid = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "newid()"),
                    Discharged = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CustomAttributesXmlSerialised = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EncounterType_Id = table.Column<int>(type: "int", nullable: false),
                    Patient_Id = table.Column<int>(type: "int", nullable: false),
                    Pregnancy_Id = table.Column<int>(type: "int", nullable: true),
                    Priority_Id = table.Column<int>(type: "int", nullable: false),
                    Archived = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    ArchivedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    ArchivedReason = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    AuditUser_Id = table.Column<int>(type: "int", nullable: true),
                    Created = table.Column<DateTime>(type: "datetime", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime", nullable: true),
                    CreatedBy_Id = table.Column<int>(type: "int", nullable: false),
                    UpdatedBy_Id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Encounter", x => x.Id);
                    table.ForeignKey(
                        name: "FK_dbo.Encounter_dbo.EncounterType_EncounterType_Id",
                        column: x => x.EncounterType_Id,
                        principalTable: "EncounterType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_dbo.Encounter_dbo.Patient_Patient_Id",
                        column: x => x.Patient_Id,
                        principalTable: "Patient",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_dbo.Encounter_dbo.Pregnancy_Pregnancy_Id",
                        column: x => x.Pregnancy_Id,
                        principalTable: "Pregnancy",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_dbo.Encounter_dbo.Priority_Priority_Id",
                        column: x => x.Priority_Id,
                        principalTable: "Priority",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_dbo.Encounter_dbo.User_AuditUser_Id",
                        column: x => x.AuditUser_Id,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_dbo.Encounter_dbo.User_CreatedBy_Id",
                        column: x => x.CreatedBy_Id,
                        principalTable: "User",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_dbo.Encounter_dbo.User_UpdatedBy_Id",
                        column: x => x.UpdatedBy_Id,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ActivityInstance",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QualifiedName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CurrentStatus_Id = table.Column<int>(type: "int", nullable: false),
                    ReportInstance_Id = table.Column<int>(type: "int", nullable: false),
                    Current = table.Column<bool>(type: "bit", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime", nullable: true),
                    CreatedBy_Id = table.Column<int>(type: "int", nullable: false),
                    UpdatedBy_Id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivityInstance", x => x.Id);
                    table.ForeignKey(
                        name: "FK_dbo.ActivityInstance_dbo.ActivityExecutionStatus_CurrentStatus_Id",
                        column: x => x.CurrentStatus_Id,
                        principalTable: "ActivityExecutionStatus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_dbo.ActivityInstance_dbo.ReportInstance_ReportInstance_Id1",
                        column: x => x.ReportInstance_Id,
                        principalTable: "ReportInstance",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_dbo.ActivityInstance_dbo.User_CreatedBy_Id",
                        column: x => x.CreatedBy_Id,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_dbo.ActivityInstance_dbo.User_UpdatedBy_Id",
                        column: x => x.UpdatedBy_Id,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DatasetXmlNode",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NodeName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NodeType = table.Column<int>(type: "int", nullable: false),
                    NodeValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ParentNode_Id = table.Column<int>(type: "int", nullable: true),
                    DatasetElement_Id = table.Column<int>(type: "int", nullable: true),
                    DatasetXml_Id = table.Column<int>(type: "int", nullable: false),
                    DatasetElementSub_Id = table.Column<int>(type: "int", nullable: true),
                    Created = table.Column<DateTime>(type: "datetime", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime", nullable: true),
                    CreatedBy_Id = table.Column<int>(type: "int", nullable: false),
                    UpdatedBy_Id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DatasetXmlNode", x => x.Id);
                    table.ForeignKey(
                        name: "FK_dbo.DatasetXmlNode_dbo.DatasetElement_DatasetElement_Id",
                        column: x => x.DatasetElement_Id,
                        principalTable: "DatasetElement",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_dbo.DatasetXmlNode_dbo.DatasetElementSub_DatasetElementSub_Id",
                        column: x => x.DatasetElementSub_Id,
                        principalTable: "DatasetElementSub",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_dbo.DatasetXmlNode_dbo.DatasetXml_DatasetXml_Id",
                        column: x => x.DatasetXml_Id,
                        principalTable: "DatasetXml",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_dbo.DatasetXmlNode_dbo.DatasetXmlNode_ParentNode_Id",
                        column: x => x.ParentNode_Id,
                        principalTable: "DatasetXmlNode",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_dbo.DatasetXmlNode_dbo.User_CreatedBy_Id",
                        column: x => x.CreatedBy_Id,
                        principalTable: "User",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_dbo.DatasetXmlNode_dbo.User_UpdatedBy_Id",
                        column: x => x.UpdatedBy_Id,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PatientClinicalEvent",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OnsetDate = table.Column<DateTime>(type: "date", nullable: true),
                    ResolutionDate = table.Column<DateTime>(type: "date", nullable: true),
                    Encounter_Id = table.Column<int>(type: "int", nullable: true),
                    Patient_Id = table.Column<int>(type: "int", nullable: false),
                    PatientClinicalEventGuid = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    SourceTerminologyMedDra_Id = table.Column<int>(type: "int", nullable: true),
                    TerminologyMedDra_Id1 = table.Column<int>(type: "int", nullable: true),
                    Archived = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    ArchivedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    ArchivedReason = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    AuditUser_Id = table.Column<int>(type: "int", nullable: true),
                    SourceDescription = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CustomAttributesXmlSerialised = table.Column<string>(type: "xml", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientClinicalEvent", x => x.Id);
                    table.ForeignKey(
                        name: "FK_dbo.PatientClinicalEvent_dbo.Encounter_Encounter_Id",
                        column: x => x.Encounter_Id,
                        principalTable: "Encounter",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_dbo.PatientClinicalEvent_dbo.Patient_Patient_Id",
                        column: x => x.Patient_Id,
                        principalTable: "Patient",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_dbo.PatientClinicalEvent_dbo.TerminologyMedDra_SourceTerminologyMedDra_Id",
                        column: x => x.SourceTerminologyMedDra_Id,
                        principalTable: "TerminologyMedDra",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_dbo.PatientClinicalEvent_dbo.User_AuditUser_Id",
                        column: x => x.AuditUser_Id,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ActivityExecutionStatusEvent",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EventDateTime = table.Column<DateTime>(type: "datetime", nullable: false),
                    Comments = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ActivityInstance_Id = table.Column<int>(type: "int", nullable: false),
                    EventCreatedBy_Id = table.Column<int>(type: "int", nullable: false),
                    ExecutionStatus_Id = table.Column<int>(type: "int", nullable: false),
                    ContextDateTime = table.Column<DateTime>(type: "datetime", nullable: false),
                    ContextCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivityExecutionStatusEvent", x => x.Id);
                    table.ForeignKey(
                        name: "FK_dbo.ActivityExecutionStatusEvent_dbo.ActivityExecutionStatus_ExecutionStatus_Id",
                        column: x => x.ExecutionStatus_Id,
                        principalTable: "ActivityExecutionStatus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_dbo.ActivityExecutionStatusEvent_dbo.ActivityInstance_ActivityInstance_Id",
                        column: x => x.ActivityInstance_Id,
                        principalTable: "ActivityInstance",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_dbo.ActivityExecutionStatusEvent_dbo.User_EventCreatedBy_Id",
                        column: x => x.EventCreatedBy_Id,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DatasetXmlAttribute",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AttributeName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AttributeValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DatasetElement_Id = table.Column<int>(type: "int", nullable: true),
                    ParentNode_Id = table.Column<int>(type: "int", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime", nullable: true),
                    CreatedBy_Id = table.Column<int>(type: "int", nullable: false),
                    UpdatedBy_Id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DatasetXmlAttribute", x => x.Id);
                    table.ForeignKey(
                        name: "FK_dbo.DatasetXmlAttribute_dbo.DatasetElement_DatasetElement_Id",
                        column: x => x.DatasetElement_Id,
                        principalTable: "DatasetElement",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_dbo.DatasetXmlAttribute_dbo.DatasetXmlNode_ParentNode_Id",
                        column: x => x.ParentNode_Id,
                        principalTable: "DatasetXmlNode",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_dbo.DatasetXmlAttribute_dbo.User_CreatedBy_Id",
                        column: x => x.CreatedBy_Id,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_dbo.DatasetXmlAttribute_dbo.User_UpdatedBy_Id",
                        column: x => x.UpdatedBy_Id,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Attachment",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Content = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    FileName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Size = table.Column<long>(type: "bigint", nullable: false),
                    AttachmentType_Id = table.Column<int>(type: "int", nullable: false),
                    Encounter_Id = table.Column<int>(type: "int", nullable: true),
                    Patient_Id = table.Column<int>(type: "int", nullable: true),
                    Archived = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    ArchivedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    ArchivedReason = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    AuditUser_Id = table.Column<int>(type: "int", nullable: true),
                    ActivityExecutionStatusEvent_Id = table.Column<int>(type: "int", nullable: true),
                    Created = table.Column<DateTime>(type: "datetime", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime", nullable: true),
                    CreatedBy_Id = table.Column<int>(type: "int", nullable: false),
                    UpdatedBy_Id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Attachment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_dbo.Attachment_dbo.ActivityExecutionStatusEvent_ActivityExecutionStatusEvent_Id",
                        column: x => x.ActivityExecutionStatusEvent_Id,
                        principalTable: "ActivityExecutionStatusEvent",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_dbo.Attachment_dbo.AttachmentType_AttachmentType_Id",
                        column: x => x.AttachmentType_Id,
                        principalTable: "AttachmentType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_dbo.Attachment_dbo.Encounter_Encounter_Id",
                        column: x => x.Encounter_Id,
                        principalTable: "Encounter",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_dbo.Attachment_dbo.Patient_Patient_Id",
                        column: x => x.Patient_Id,
                        principalTable: "Patient",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_dbo.Attachment_dbo.User_AuditUser_Id",
                        column: x => x.AuditUser_Id,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_dbo.Attachment_dbo.User_CreatedBy_Id",
                        column: x => x.CreatedBy_Id,
                        principalTable: "User",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_dbo.Attachment_dbo.User_UpdatedBy_Id",
                        column: x => x.UpdatedBy_Id,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "WorkPlanCareEvent",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Order = table.Column<short>(type: "smallint", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    CareEvent_Id = table.Column<int>(type: "int", nullable: false),
                    WorkPlan_Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkPlanCareEvent", x => x.Id);
                    table.ForeignKey(
                        name: "FK_dbo.WorkPlanCareEvent_dbo.CareEvent_CareEvent_Id",
                        column: x => x.CareEvent_Id,
                        principalTable: "CareEvent",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EncounterTypeWorkPlan",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CohortGroup_Id = table.Column<int>(type: "int", nullable: true),
                    EncounterType_Id = table.Column<int>(type: "int", nullable: false),
                    WorkPlan_Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EncounterTypeWorkPlan", x => x.Id);
                    table.ForeignKey(
                        name: "FK_dbo.EncounterTypeWorkPlan_dbo.CohortGroup_CohortGroup_Id",
                        column: x => x.CohortGroup_Id,
                        principalTable: "CohortGroup",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_dbo.EncounterTypeWorkPlan_dbo.EncounterType_EncounterType_Id",
                        column: x => x.EncounterType_Id,
                        principalTable: "EncounterType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Dataset",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DatasetName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    InitialiseProcess = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    RulesProcess = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Help = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    ContextType_Id = table.Column<int>(type: "int", nullable: false),
                    EncounterTypeWorkPlan_Id = table.Column<int>(type: "int", nullable: true),
                    UID = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    IsSystem = table.Column<bool>(type: "bit", nullable: false),
                    DatasetXml_Id = table.Column<int>(type: "int", nullable: true),
                    Created = table.Column<DateTime>(type: "datetime", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime", nullable: true),
                    CreatedBy_Id = table.Column<int>(type: "int", nullable: false),
                    UpdatedBy_Id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dataset", x => x.Id);
                    table.ForeignKey(
                        name: "FK_dbo.Dataset_dbo.ContextType_ContextType_Id",
                        column: x => x.ContextType_Id,
                        principalTable: "ContextTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_dbo.Dataset_dbo.DatasetXml_DatasetXml_Id",
                        column: x => x.DatasetXml_Id,
                        principalTable: "DatasetXml",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_dbo.Dataset_dbo.EncounterTypeWorkPlan_EncounterTypeWorkPlan_Id",
                        column: x => x.EncounterTypeWorkPlan_Id,
                        principalTable: "EncounterTypeWorkPlan",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_dbo.Dataset_dbo.User_CreatedBy_Id",
                        column: x => x.CreatedBy_Id,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_dbo.Dataset_dbo.User_UpdatedBy_Id",
                        column: x => x.UpdatedBy_Id,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DatasetCategory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DatasetCategoryName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CategoryOrder = table.Column<short>(type: "smallint", nullable: false),
                    Dataset_Id = table.Column<int>(type: "int", nullable: false),
                    UID = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    System = table.Column<bool>(type: "bit", nullable: false),
                    Acute = table.Column<bool>(type: "bit", nullable: false),
                    Chronic = table.Column<bool>(type: "bit", nullable: false),
                    Public = table.Column<bool>(type: "bit", nullable: false),
                    FriendlyName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Help = table.Column<string>(type: "nvarchar(350)", maxLength: 350, nullable: true),
                    Dataset_Id1 = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DatasetCategory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_dbo.DatasetCategory_dbo.Dataset_Dataset_Id1",
                        column: x => x.Dataset_Id,
                        principalTable: "Dataset",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DatasetInstance",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ContextID = table.Column<int>(type: "int", nullable: false),
                    Dataset_Id = table.Column<int>(type: "int", nullable: false),
                    EncounterTypeWorkPlan_Id = table.Column<int>(type: "int", nullable: true),
                    Tag = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DatasetInstanceGuid = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "newid()"),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime", nullable: true),
                    CreatedBy_Id = table.Column<int>(type: "int", nullable: false),
                    UpdatedBy_Id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DatasetInstance", x => x.Id);
                    table.ForeignKey(
                        name: "FK_dbo.DatasetInstance_dbo.Dataset_Dataset_Id",
                        column: x => x.Dataset_Id,
                        principalTable: "Dataset",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_dbo.DatasetInstance_dbo.EncounterTypeWorkPlan_EncounterTypeWorkPlan_Id",
                        column: x => x.EncounterTypeWorkPlan_Id,
                        principalTable: "EncounterTypeWorkPlan",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_dbo.DatasetInstance_dbo.User_CreatedBy_Id",
                        column: x => x.CreatedBy_Id,
                        principalTable: "User",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_dbo.DatasetInstance_dbo.User_UpdatedBy_Id",
                        column: x => x.UpdatedBy_Id,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "DatasetRule",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RuleType = table.Column<int>(type: "int", nullable: false),
                    RuleActive = table.Column<bool>(type: "bit", nullable: false),
                    Dataset_Id = table.Column<int>(type: "int", nullable: true),
                    DatasetElement_Id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DatasetRule", x => x.Id);
                    table.ForeignKey(
                        name: "FK_dbo.DatasetRule_dbo.Dataset_Dataset_Id",
                        column: x => x.Dataset_Id,
                        principalTable: "Dataset",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_dbo.DatasetRule_dbo.DatasetElement_DatasetElement_Id",
                        column: x => x.DatasetElement_Id,
                        principalTable: "DatasetElement",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkPlan",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Dataset_Id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkPlan", x => x.Id);
                    table.ForeignKey(
                        name: "FK_dbo.WorkPlan_dbo.Dataset_Dataset_Id",
                        column: x => x.Dataset_Id,
                        principalTable: "Dataset",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DatasetCategoryCondition",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Condition_Id = table.Column<int>(type: "int", nullable: false),
                    DatasetCategory_Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DatasetCategoryCondition", x => x.Id);
                    table.ForeignKey(
                        name: "FK_dbo.DatasetCategoryCondition_dbo.Condition_Condition_Id",
                        column: x => x.Condition_Id,
                        principalTable: "Condition",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_dbo.DatasetCategoryCondition_dbo.DatasetCategory_DatasetCategory_Id",
                        column: x => x.DatasetCategory_Id,
                        principalTable: "DatasetCategory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DatasetCategoryElement",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FieldOrder = table.Column<short>(type: "smallint", nullable: false),
                    DatasetCategory_Id = table.Column<int>(type: "int", nullable: false),
                    DatasetElement_Id = table.Column<int>(type: "int", nullable: false),
                    Acute = table.Column<bool>(type: "bit", nullable: false),
                    Chronic = table.Column<bool>(type: "bit", nullable: false),
                    UID = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    System = table.Column<bool>(type: "bit", nullable: false),
                    Public = table.Column<bool>(type: "bit", nullable: false),
                    FriendlyName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Help = table.Column<string>(type: "nvarchar(350)", maxLength: 350, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DatasetCategoryElement", x => x.Id);
                    table.ForeignKey(
                        name: "FK_dbo.DatasetCategoryElement_dbo.DatasetCategory_DatasetCategory_Id",
                        column: x => x.DatasetCategory_Id,
                        principalTable: "DatasetCategory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_dbo.DatasetCategoryElement_dbo.DatasetElement_DatasetElement_Id",
                        column: x => x.DatasetElement_Id,
                        principalTable: "DatasetElement",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkPlanCareEventDatasetCategory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DatasetCategory_Id = table.Column<int>(type: "int", nullable: false),
                    WorkPlanCareEvent_Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkPlanCareEventDatasetCategory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_dbo.WorkPlanCareEventDatasetCategory_dbo.DatasetCategory_DatasetCategory_Id",
                        column: x => x.DatasetCategory_Id,
                        principalTable: "DatasetCategory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_dbo.WorkPlanCareEventDatasetCategory_dbo.WorkPlanCareEvent_WorkPlanCareEvent_Id",
                        column: x => x.WorkPlanCareEvent_Id,
                        principalTable: "WorkPlanCareEvent",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DatasetInstanceValue",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InstanceValue = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DatasetElement_Id = table.Column<int>(type: "int", nullable: false),
                    DatasetInstance_Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DatasetInstanceValue", x => x.Id);
                    table.ForeignKey(
                        name: "FK_dbo.DatasetInstanceValue_dbo.DatasetElement_DatasetElement_Id",
                        column: x => x.DatasetElement_Id,
                        principalTable: "DatasetElement",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_dbo.DatasetInstanceValue_dbo.DatasetInstance_DatasetInstance_Id",
                        column: x => x.DatasetInstance_Id,
                        principalTable: "DatasetInstance",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DatasetCategoryElementCondition",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Condition_Id = table.Column<int>(type: "int", nullable: false),
                    DatasetCategoryElement_Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DatasetCategoryElementCondition", x => x.Id);
                    table.ForeignKey(
                        name: "FK_dbo.DatasetCategoryElementCondition_dbo.Condition_Condition_Id",
                        column: x => x.Condition_Id,
                        principalTable: "Condition",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_dbo.DatasetCategoryElementCondition_dbo.DatasetCategoryElement_DatasetCategoryElement_Id",
                        column: x => x.DatasetCategoryElement_Id,
                        principalTable: "DatasetCategoryElement",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DatasetMapping",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Tag = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MappingType = table.Column<int>(type: "int", nullable: false),
                    MappingOption = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DestinationElement_Id = table.Column<int>(type: "int", nullable: false),
                    SourceElement_Id = table.Column<int>(type: "int", nullable: true),
                    PropertyPath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Property = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DatasetMapping", x => x.Id);
                    table.ForeignKey(
                        name: "FK_dbo.DatasetMapping_dbo.DatasetCategoryElement_DestinationElement_Id",
                        column: x => x.DestinationElement_Id,
                        principalTable: "DatasetCategoryElement",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_dbo.DatasetMapping_dbo.DatasetCategoryElement_SourceElement_Id",
                        column: x => x.SourceElement_Id,
                        principalTable: "DatasetCategoryElement",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "DatasetInstanceSubValue",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ContextValue = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InstanceValue = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DatasetElementSub_Id = table.Column<int>(type: "int", nullable: false),
                    DatasetInstanceValue_Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DatasetInstanceSubValue", x => x.Id);
                    table.ForeignKey(
                        name: "FK_dbo.DatasetInstanceSubValue_dbo.DatasetElementSub_DatasetElementSub_Id",
                        column: x => x.DatasetElementSub_Id,
                        principalTable: "DatasetElementSub",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_dbo.DatasetInstanceSubValue_dbo.DatasetInstanceValue_DatasetInstanceValue_Id",
                        column: x => x.DatasetInstanceValue_Id,
                        principalTable: "DatasetInstanceValue",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "DatasetMappingSub",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PropertyPath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Property = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MappingType = table.Column<int>(type: "int", nullable: false),
                    MappingOption = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DestinationElement_Id = table.Column<int>(type: "int", nullable: false),
                    Mapping_Id = table.Column<int>(type: "int", nullable: false),
                    SourceElement_Id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DatasetMappingSub", x => x.Id);
                    table.ForeignKey(
                        name: "FK_dbo.DatasetMappingSub_dbo.DatasetElementSub_DestinationElement_Id",
                        column: x => x.DestinationElement_Id,
                        principalTable: "DatasetElementSub",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_dbo.DatasetMappingSub_dbo.DatasetElementSub_SourceElement_Id",
                        column: x => x.SourceElement_Id,
                        principalTable: "DatasetElementSub",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_dbo.DatasetMappingSub_dbo.DatasetMapping_Mapping_Id",
                        column: x => x.Mapping_Id,
                        principalTable: "DatasetMapping",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "DatasetMappingValue",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SourceValue = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DestinationValue = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    Mapping_Id = table.Column<int>(type: "int", nullable: false),
                    SubMapping_Id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DatasetMappingValue", x => x.Id);
                    table.ForeignKey(
                        name: "FK_dbo.DatasetMappingValue_dbo.DatasetMapping_Mapping_Id",
                        column: x => x.Mapping_Id,
                        principalTable: "DatasetMapping",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_dbo.DatasetMappingValue_dbo.DatasetMappingSub_SubMapping_Id",
                        column: x => x.SubMapping_Id,
                        principalTable: "DatasetMappingSub",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Activity_QualifiedName_WorkFlow_Id",
                table: "Activity",
                columns: new[] { "QualifiedName", "WorkFlow_Id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WorkFlow_Id",
                table: "Activity",
                column: "WorkFlow_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Activity_Id",
                table: "ActivityExecutionStatus",
                column: "Activity_Id");

            migrationBuilder.CreateIndex(
                name: "IX_ActivityExecutionStatus_Description_Activity_Id",
                table: "ActivityExecutionStatus",
                columns: new[] { "Description", "Activity_Id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ActivityExecutionStatusEvent_ActivityInstance_Id_ExecutionStatus_Id",
                table: "ActivityExecutionStatusEvent",
                columns: new[] { "ActivityInstance_Id", "ExecutionStatus_Id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ActivityInstance_Id",
                table: "ActivityExecutionStatusEvent",
                column: "ActivityInstance_Id");

            migrationBuilder.CreateIndex(
                name: "IX_EventCreatedBy_Id",
                table: "ActivityExecutionStatusEvent",
                column: "EventCreatedBy_Id");

            migrationBuilder.CreateIndex(
                name: "IX_ExecutionStatus_Id",
                table: "ActivityExecutionStatusEvent",
                column: "ExecutionStatus_Id");

            migrationBuilder.CreateIndex(
                name: "IX_ActivityInstance_QualifiedName_ReportInstance_Id",
                table: "ActivityInstance",
                columns: new[] { "QualifiedName", "ReportInstance_Id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CreatedBy_Id",
                table: "ActivityInstance",
                column: "CreatedBy_Id");

            migrationBuilder.CreateIndex(
                name: "IX_CurrentStatus_Id",
                table: "ActivityInstance",
                column: "CurrentStatus_Id");

            migrationBuilder.CreateIndex(
                name: "IX_ReportInstance_Id",
                table: "ActivityInstance",
                column: "ReportInstance_Id");

            migrationBuilder.CreateIndex(
                name: "IX_UpdatedBy_Id",
                table: "ActivityInstance",
                column: "UpdatedBy_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Appointment_AppointmentDate",
                table: "Appointment",
                column: "AppointmentDate");

            migrationBuilder.CreateIndex(
                name: "IX_AuditUser_Id",
                table: "Appointment",
                column: "AuditUser_Id");

            migrationBuilder.CreateIndex(
                name: "IX_CreatedBy_Id1",
                table: "Appointment",
                column: "CreatedBy_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Patient_Id",
                table: "Appointment",
                column: "Patient_Id");

            migrationBuilder.CreateIndex(
                name: "IX_UpdatedBy_Id1",
                table: "Appointment",
                column: "UpdatedBy_Id");

            migrationBuilder.CreateIndex(
                name: "IX_ActivityExecutionStatusEvent_Id",
                table: "Attachment",
                column: "ActivityExecutionStatusEvent_Id");

            migrationBuilder.CreateIndex(
                name: "IX_AttachmentType_Id",
                table: "Attachment",
                column: "AttachmentType_Id");

            migrationBuilder.CreateIndex(
                name: "IX_AuditUser_Id1",
                table: "Attachment",
                column: "AuditUser_Id");

            migrationBuilder.CreateIndex(
                name: "IX_CreatedBy_Id2",
                table: "Attachment",
                column: "CreatedBy_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Encounter_Id",
                table: "Attachment",
                column: "Encounter_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Patient_Id1",
                table: "Attachment",
                column: "Patient_Id");

            migrationBuilder.CreateIndex(
                name: "IX_UpdatedBy_Id2",
                table: "Attachment",
                column: "UpdatedBy_Id");

            migrationBuilder.CreateIndex(
                name: "IX_AttachmentType_Description",
                table: "AttachmentType",
                column: "Description",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AttachmentType_Key",
                table: "AttachmentType",
                column: "Key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AuditLog_ActionDate",
                table: "AuditLog",
                column: "ActionDate");

            migrationBuilder.CreateIndex(
                name: "IX_User_Id3",
                table: "AuditLog",
                column: "User_Id");

            migrationBuilder.CreateIndex(
                name: "IX_CareEvent_Description",
                table: "CareEvent",
                column: "Description",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CohortGroup_CohortCode",
                table: "CohortGroup",
                column: "CohortCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CohortGroup_CohortName",
                table: "CohortGroup",
                column: "CohortName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Condition_Id",
                table: "CohortGroup",
                column: "Condition_Id");

            migrationBuilder.CreateIndex(
                name: "IX_AuditUser_Id2",
                table: "CohortGroupEnrolment",
                column: "AuditUser_Id");

            migrationBuilder.CreateIndex(
                name: "IX_CohortGroup_Id",
                table: "CohortGroupEnrolment",
                column: "CohortGroup_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Patient_Id2",
                table: "CohortGroupEnrolment",
                column: "Patient_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Concept_ConceptName_MedicationForm_Id",
                table: "Concept",
                columns: new[] { "ConceptName", "MedicationForm_Id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MedicationForm_Id",
                table: "Concept",
                column: "MedicationForm_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Concept_Id",
                table: "ConceptIngredient",
                column: "Concept_Id");

            migrationBuilder.CreateIndex(
                name: "IX_ConceptIngredient_Concept_Id_Ingredient_Strength",
                table: "ConceptIngredient",
                columns: new[] { "Concept_Id", "Ingredient", "Strength" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Condition_Description",
                table: "Condition",
                column: "Description",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Condition_Id1",
                table: "ConditionLabTest",
                column: "Condition_Id");

            migrationBuilder.CreateIndex(
                name: "IX_ConditionLabTest_Condition_Id_LabTest_Id",
                table: "ConditionLabTest",
                columns: new[] { "Condition_Id", "LabTest_Id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LabTest_Id",
                table: "ConditionLabTest",
                column: "LabTest_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Condition_Id2",
                table: "ConditionMedDra",
                column: "Condition_Id");

            migrationBuilder.CreateIndex(
                name: "IX_ConditionMedDra_Condition_Id_TerminologyMedDra_Id",
                table: "ConditionMedDra",
                columns: new[] { "Condition_Id", "TerminologyMedDra_Id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TerminologyMedDra_Id",
                table: "ConditionMedDra",
                column: "TerminologyMedDra_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Concept_Id1",
                table: "ConditionMedication",
                column: "Concept_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Condition_Id3",
                table: "ConditionMedication",
                column: "Condition_Id");

            migrationBuilder.CreateIndex(
                name: "IX_ConditionMedication_Condition_Id_Concept_Id_Product_Id",
                table: "ConditionMedication",
                columns: new[] { "Condition_Id", "Concept_Id", "Product_Id" },
                unique: true,
                filter: "[Product_Id] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Product_Id",
                table: "ConditionMedication",
                column: "Product_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Config_ConfigType",
                table: "Config",
                column: "ConfigType");

            migrationBuilder.CreateIndex(
                name: "IX_CreatedBy_Id3",
                table: "Config",
                column: "CreatedBy_Id");

            migrationBuilder.CreateIndex(
                name: "IX_UpdatedBy_Id3",
                table: "Config",
                column: "UpdatedBy_Id");

            migrationBuilder.CreateIndex(
                name: "IX_CustomAttributeConfiguration_ExtendableTypeName_CustomAttributeType_AttributeKey",
                table: "CustomAttributeConfiguration",
                columns: new[] { "ExtendableTypeName", "CustomAttributeType", "AttributeKey" },
                unique: true,
                filter: "[AttributeKey] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ContextType_Id",
                table: "Dataset",
                column: "ContextType_Id");

            migrationBuilder.CreateIndex(
                name: "IX_CreatedBy_Id4",
                table: "Dataset",
                column: "CreatedBy_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Dataset_DatasetName",
                table: "Dataset",
                column: "DatasetName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DatasetXml_Id",
                table: "Dataset",
                column: "DatasetXml_Id");

            migrationBuilder.CreateIndex(
                name: "IX_EncounterTypeWorkPlan_Id",
                table: "Dataset",
                column: "EncounterTypeWorkPlan_Id");

            migrationBuilder.CreateIndex(
                name: "IX_UpdatedBy_Id4",
                table: "Dataset",
                column: "UpdatedBy_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Dataset_Id",
                table: "DatasetCategory",
                column: "Dataset_Id");

            migrationBuilder.CreateIndex(
                name: "IX_DatasetCategory_Dataset_Id1_DatasetCategoryName",
                table: "DatasetCategory",
                columns: new[] { "Dataset_Id1", "DatasetCategoryName" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Condition_Id4",
                table: "DatasetCategoryCondition",
                column: "Condition_Id");

            migrationBuilder.CreateIndex(
                name: "IX_DatasetCategory_Id",
                table: "DatasetCategoryCondition",
                column: "DatasetCategory_Id");

            migrationBuilder.CreateIndex(
                name: "IX_DatasetCategoryCondition_Condition_Id_DatasetCategory_Id",
                table: "DatasetCategoryCondition",
                columns: new[] { "Condition_Id", "DatasetCategory_Id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DatasetCategory_Id1",
                table: "DatasetCategoryElement",
                column: "DatasetCategory_Id");

            migrationBuilder.CreateIndex(
                name: "IX_DatasetCategoryElement_DatasetCategory_Id_DatasetElement_Id",
                table: "DatasetCategoryElement",
                columns: new[] { "DatasetCategory_Id", "DatasetElement_Id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DatasetElement_Id",
                table: "DatasetCategoryElement",
                column: "DatasetElement_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Condition_Id5",
                table: "DatasetCategoryElementCondition",
                column: "Condition_Id");

            migrationBuilder.CreateIndex(
                name: "IX_DatasetCategoryElement_Id",
                table: "DatasetCategoryElementCondition",
                column: "DatasetCategoryElement_Id");

            migrationBuilder.CreateIndex(
                name: "IX_DatasetCategoryElementCondition_Condition_Id_DatasetCategoryElement_Id",
                table: "DatasetCategoryElementCondition",
                columns: new[] { "Condition_Id", "DatasetCategoryElement_Id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DatasetElement_ElementName",
                table: "DatasetElement",
                column: "ElementName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DatasetElementType_Id",
                table: "DatasetElement",
                column: "DatasetElementType_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Field_Id",
                table: "DatasetElement",
                column: "Field_Id");

            migrationBuilder.CreateIndex(
                name: "IX_DatasetElement_Id1",
                table: "DatasetElementSub",
                column: "DatasetElement_Id");

            migrationBuilder.CreateIndex(
                name: "IX_DatasetElementSub_DatasetElement_Id_ElementName",
                table: "DatasetElementSub",
                columns: new[] { "DatasetElement_Id", "ElementName" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Field_Id1",
                table: "DatasetElementSub",
                column: "Field_Id");

            migrationBuilder.CreateIndex(
                name: "IX_DatasetElementType_Description",
                table: "DatasetElementType",
                column: "Description",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CreatedBy_Id5",
                table: "DatasetInstance",
                column: "CreatedBy_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Dataset_Id1",
                table: "DatasetInstance",
                column: "Dataset_Id");

            migrationBuilder.CreateIndex(
                name: "IX_EncounterTypeWorkPlan_Id1",
                table: "DatasetInstance",
                column: "EncounterTypeWorkPlan_Id");

            migrationBuilder.CreateIndex(
                name: "IX_UpdatedBy_Id5",
                table: "DatasetInstance",
                column: "UpdatedBy_Id");

            migrationBuilder.CreateIndex(
                name: "IX_DatasetElementSub_Id",
                table: "DatasetInstanceSubValue",
                column: "DatasetElementSub_Id");

            migrationBuilder.CreateIndex(
                name: "IX_DatasetInstanceSubValue_DatasetInstanceValue_Id_DatasetElementSub_Id",
                table: "DatasetInstanceSubValue",
                columns: new[] { "DatasetInstanceValue_Id", "DatasetElementSub_Id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DatasetInstanceValue_Id",
                table: "DatasetInstanceSubValue",
                column: "DatasetInstanceValue_Id");

            migrationBuilder.CreateIndex(
                name: "IX_DatasetElement_Id2",
                table: "DatasetInstanceValue",
                column: "DatasetElement_Id");

            migrationBuilder.CreateIndex(
                name: "IX_DatasetInstance_Id",
                table: "DatasetInstanceValue",
                column: "DatasetInstance_Id");

            migrationBuilder.CreateIndex(
                name: "IX_DatasetInstanceValue_DatasetInstance_Id_DatasetElement_Id",
                table: "DatasetInstanceValue",
                columns: new[] { "DatasetInstance_Id", "DatasetElement_Id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DestinationElement_Id",
                table: "DatasetMapping",
                column: "DestinationElement_Id");

            migrationBuilder.CreateIndex(
                name: "IX_SourceElement_Id",
                table: "DatasetMapping",
                column: "SourceElement_Id");

            migrationBuilder.CreateIndex(
                name: "IX_DestinationElement_Id1",
                table: "DatasetMappingSub",
                column: "DestinationElement_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Mapping_Id",
                table: "DatasetMappingSub",
                column: "Mapping_Id");

            migrationBuilder.CreateIndex(
                name: "IX_SourceElement_Id1",
                table: "DatasetMappingSub",
                column: "SourceElement_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Mapping_Id1",
                table: "DatasetMappingValue",
                column: "Mapping_Id");

            migrationBuilder.CreateIndex(
                name: "IX_SubMapping_Id",
                table: "DatasetMappingValue",
                column: "SubMapping_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Dataset_Id2",
                table: "DatasetRule",
                column: "Dataset_Id");

            migrationBuilder.CreateIndex(
                name: "IX_DatasetElement_Id3",
                table: "DatasetRule",
                column: "DatasetElement_Id");

            migrationBuilder.CreateIndex(
                name: "IX_CreatedBy_Id6",
                table: "DatasetXml",
                column: "CreatedBy_Id");

            migrationBuilder.CreateIndex(
                name: "IX_UpdatedBy_Id6",
                table: "DatasetXml",
                column: "UpdatedBy_Id");

            migrationBuilder.CreateIndex(
                name: "IX_CreatedBy_Id7",
                table: "DatasetXmlAttribute",
                column: "CreatedBy_Id");

            migrationBuilder.CreateIndex(
                name: "IX_DatasetElement_Id4",
                table: "DatasetXmlAttribute",
                column: "DatasetElement_Id");

            migrationBuilder.CreateIndex(
                name: "IX_ParentNode_Id",
                table: "DatasetXmlAttribute",
                column: "ParentNode_Id");

            migrationBuilder.CreateIndex(
                name: "IX_UpdatedBy_Id7",
                table: "DatasetXmlAttribute",
                column: "UpdatedBy_Id");

            migrationBuilder.CreateIndex(
                name: "IX_CreatedBy_Id8",
                table: "DatasetXmlNode",
                column: "CreatedBy_Id");

            migrationBuilder.CreateIndex(
                name: "IX_DatasetElement_Id5",
                table: "DatasetXmlNode",
                column: "DatasetElement_Id");

            migrationBuilder.CreateIndex(
                name: "IX_DatasetElementSub_Id1",
                table: "DatasetXmlNode",
                column: "DatasetElementSub_Id");

            migrationBuilder.CreateIndex(
                name: "IX_DatasetXml_Id1",
                table: "DatasetXmlNode",
                column: "DatasetXml_Id");

            migrationBuilder.CreateIndex(
                name: "IX_DatasetXmlNode_DatasetXml_Id_NodeName",
                table: "DatasetXmlNode",
                columns: new[] { "DatasetXml_Id", "NodeName" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ParentNode_Id1",
                table: "DatasetXmlNode",
                column: "ParentNode_Id");

            migrationBuilder.CreateIndex(
                name: "IX_UpdatedBy_Id8",
                table: "DatasetXmlNode",
                column: "UpdatedBy_Id");

            migrationBuilder.CreateIndex(
                name: "IX_AuditUser_Id3",
                table: "Encounter",
                column: "AuditUser_Id");

            migrationBuilder.CreateIndex(
                name: "IX_CreatedBy_Id9",
                table: "Encounter",
                column: "CreatedBy_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Encounter_EncounterDate",
                table: "Encounter",
                column: "EncounterDate");

            migrationBuilder.CreateIndex(
                name: "IX_Encounter_Patient_Id_EncounterDate",
                table: "Encounter",
                columns: new[] { "Patient_Id", "EncounterDate" });

            migrationBuilder.CreateIndex(
                name: "IX_EncounterType_Id",
                table: "Encounter",
                column: "EncounterType_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Patient_Id3",
                table: "Encounter",
                column: "Patient_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Pregnancy_Id",
                table: "Encounter",
                column: "Pregnancy_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Priority_Id",
                table: "Encounter",
                column: "Priority_Id");

            migrationBuilder.CreateIndex(
                name: "IX_UpdatedBy_Id9",
                table: "Encounter",
                column: "UpdatedBy_Id");

            migrationBuilder.CreateIndex(
                name: "IX_EncounterType_Description",
                table: "EncounterType",
                column: "Description",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CohortGroup_Id1",
                table: "EncounterTypeWorkPlan",
                column: "CohortGroup_Id");

            migrationBuilder.CreateIndex(
                name: "IX_EncounterType_Id1",
                table: "EncounterTypeWorkPlan",
                column: "EncounterType_Id");

            migrationBuilder.CreateIndex(
                name: "IX_EncounterTypeWorkPlan_CohortGroup_Id_EncounterType_Id_WorkPlan_Id",
                table: "EncounterTypeWorkPlan",
                columns: new[] { "CohortGroup_Id", "EncounterType_Id", "WorkPlan_Id" },
                unique: true,
                filter: "[CohortGroup_Id] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_WorkPlan_Id",
                table: "EncounterTypeWorkPlan",
                column: "WorkPlan_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Facility_FacilityCode",
                table: "Facility",
                column: "FacilityCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Facility_FacilityName",
                table: "Facility",
                column: "FacilityName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FacilityType_Id",
                table: "Facility",
                column: "FacilityType_Id");

            migrationBuilder.CreateIndex(
                name: "IX_OrgUnit_Id",
                table: "Facility",
                column: "OrgUnit_Id");

            migrationBuilder.CreateIndex(
                name: "IX_FacilityType_Description",
                table: "FacilityType",
                column: "Description",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FieldType_Id",
                table: "Field",
                column: "FieldType_Id");

            migrationBuilder.CreateIndex(
                name: "IX_FieldType_Description",
                table: "FieldType",
                column: "Description",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Field_Id2",
                table: "FieldValue",
                column: "Field_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Holiday_HolidayDate",
                table: "Holiday",
                column: "HolidayDate",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LabResult_Description",
                table: "LabResult",
                column: "Description",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LabTest_Description",
                table: "LabTest",
                column: "Description",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LabTestUnit_Description",
                table: "LabTestUnit",
                column: "Description",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Language_Description",
                table: "Language",
                column: "Description",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Scale_Id",
                table: "MedDRAGrading",
                column: "Scale_Id");

            migrationBuilder.CreateIndex(
                name: "IX_GradingScale_Id",
                table: "MedDRAScale",
                column: "GradingScale_Id");

            migrationBuilder.CreateIndex(
                name: "IX_TerminologyMedDra_Id1",
                table: "MedDRAScale",
                column: "TerminologyMedDra_Id");

            migrationBuilder.CreateIndex(
                name: "IX_MedicationForm_Description",
                table: "MedicationForm",
                column: "Description",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ColumnType_Id",
                table: "MetaColumn",
                column: "ColumnType_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Table_Id",
                table: "MetaColumn",
                column: "Table_Id");

            migrationBuilder.CreateIndex(
                name: "IX_MetaColumnType_Description",
                table: "MetaColumnType",
                column: "Description",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ParentTable_Id",
                table: "MetaDependency",
                column: "ParentTable_Id");

            migrationBuilder.CreateIndex(
                name: "IX_ReferenceTable_Id",
                table: "MetaDependency",
                column: "ReferenceTable_Id");

            migrationBuilder.CreateIndex(
                name: "IX_MetaForm_FormName",
                table: "MetaForm",
                column: "FormName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MetaPage_PageName",
                table: "MetaPage",
                column: "PageName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MetaReport_ReportName",
                table: "MetaReport",
                column: "ReportName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MetaTable_TableName",
                table: "MetaTable",
                column: "TableName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TableType_Id",
                table: "MetaTable",
                column: "TableType_Id");

            migrationBuilder.CreateIndex(
                name: "IX_MetaTableType_Description",
                table: "MetaTableType",
                column: "Description",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MetaPage_Id",
                table: "MetaWidget",
                column: "MetaPage_Id");

            migrationBuilder.CreateIndex(
                name: "IX_MetaWidget_WidgetName",
                table: "MetaWidget",
                column: "WidgetName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WidgetType_Id",
                table: "MetaWidget",
                column: "WidgetType_Id");

            migrationBuilder.CreateIndex(
                name: "IX_MetaWidgetType_Description",
                table: "MetaWidgetType",
                column: "Description",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrgUnit_Name",
                table: "OrgUnit",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrgUnitType_Id",
                table: "OrgUnit",
                column: "OrgUnitType_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Parent_Id",
                table: "OrgUnit",
                column: "Parent_Id");

            migrationBuilder.CreateIndex(
                name: "IX_OrgUnitTypes_ParentId",
                table: "OrgUnitTypes",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_Outcome_Description",
                table: "Outcome",
                column: "Description",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AuditUser_Id4",
                table: "Patient",
                column: "AuditUser_Id");

            migrationBuilder.CreateIndex(
                name: "IX_CreatedBy_Id10",
                table: "Patient",
                column: "CreatedBy_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Patient_Created",
                table: "Patient",
                column: "Created");

            migrationBuilder.CreateIndex(
                name: "IX_Patient_DateOfBirth",
                table: "Patient",
                column: "DateOfBirth");

            migrationBuilder.CreateIndex(
                name: "IX_Patient_Surname_FirstName",
                table: "Patient",
                columns: new[] { "Surname", "FirstName" });

            migrationBuilder.CreateIndex(
                name: "IX_UpdatedBy_Id10",
                table: "Patient",
                column: "UpdatedBy_Id");

            migrationBuilder.CreateIndex(
                name: "IX_AuditUser_Id5",
                table: "PatientClinicalEvent",
                column: "AuditUser_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Encounter_Id1",
                table: "PatientClinicalEvent",
                column: "Encounter_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Patient_Id4",
                table: "PatientClinicalEvent",
                column: "Patient_Id");

            migrationBuilder.CreateIndex(
                name: "IX_SourceTerminologyMedDra_Id",
                table: "PatientClinicalEvent",
                column: "SourceTerminologyMedDra_Id");

            migrationBuilder.CreateIndex(
                name: "IX_AuditUser_Id6",
                table: "PatientCondition",
                column: "AuditUser_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Condition_Id6",
                table: "PatientCondition",
                column: "Condition_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Outcome_Id",
                table: "PatientCondition",
                column: "Outcome_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Patient_Id5",
                table: "PatientCondition",
                column: "Patient_Id");

            migrationBuilder.CreateIndex(
                name: "IX_TerminologyMedDra_Id2",
                table: "PatientCondition",
                column: "TerminologyMedDra_Id");

            migrationBuilder.CreateIndex(
                name: "IX_TreatmentOutcome_Id",
                table: "PatientCondition",
                column: "TreatmentOutcome_Id");

            migrationBuilder.CreateIndex(
                name: "IX_AuditUser_Id7",
                table: "PatientFacility",
                column: "AuditUser_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Facility_Id1",
                table: "PatientFacility",
                column: "Facility_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Patient_Id6",
                table: "PatientFacility",
                column: "Patient_Id");

            migrationBuilder.CreateIndex(
                name: "IX_PatientFacility_Patient_Id_Facility_Id",
                table: "PatientFacility",
                columns: new[] { "Patient_Id", "Facility_Id" });

            migrationBuilder.CreateIndex(
                name: "IX_AuditUser_Id8",
                table: "PatientLabTest",
                column: "AuditUser_Id");

            migrationBuilder.CreateIndex(
                name: "IX_LabTest_Id1",
                table: "PatientLabTest",
                column: "LabTest_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Patient_Id7",
                table: "PatientLabTest",
                column: "Patient_Id");

            migrationBuilder.CreateIndex(
                name: "IX_PatientLabTest_Patient_Id_LabTest_Id",
                table: "PatientLabTest",
                columns: new[] { "Patient_Id", "LabTest_Id" });

            migrationBuilder.CreateIndex(
                name: "IX_TestUnit_Id",
                table: "PatientLabTest",
                column: "TestUnit_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Language_Id",
                table: "PatientLanguage",
                column: "Language_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Patient_Id8",
                table: "PatientLanguage",
                column: "Patient_Id");

            migrationBuilder.CreateIndex(
                name: "IX_PatientLanguage_Patient_Id_Language_Id",
                table: "PatientLanguage",
                columns: new[] { "Patient_Id", "Language_Id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AuditUser_Id9",
                table: "PatientMedication",
                column: "AuditUser_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Concept_Id2",
                table: "PatientMedication",
                column: "Concept_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Patient_Id9",
                table: "PatientMedication",
                column: "Patient_Id");

            migrationBuilder.CreateIndex(
                name: "IX_PatientMedication_Patient_Id_Concept_Id_Product_Id",
                table: "PatientMedication",
                columns: new[] { "Patient_Id", "Concept_Id", "Product_Id" });

            migrationBuilder.CreateIndex(
                name: "IX_Product_Id1",
                table: "PatientMedication",
                column: "Product_Id");

            migrationBuilder.CreateIndex(
                name: "IX_PatientStatus_Description",
                table: "PatientStatus",
                column: "Description");

            migrationBuilder.CreateIndex(
                name: "IX_AuditUser_Id10",
                table: "PatientStatusHistory",
                column: "AuditUser_Id");

            migrationBuilder.CreateIndex(
                name: "IX_CreatedBy_Id11",
                table: "PatientStatusHistory",
                column: "CreatedBy_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Patient_Id10",
                table: "PatientStatusHistory",
                column: "Patient_Id");

            migrationBuilder.CreateIndex(
                name: "IX_PatientStatus_Id",
                table: "PatientStatusHistory",
                column: "PatientStatus_Id");

            migrationBuilder.CreateIndex(
                name: "IX_PatientStatusHistory_Patient_Id_PatientStatus_Id",
                table: "PatientStatusHistory",
                columns: new[] { "Patient_Id", "PatientStatus_Id" });

            migrationBuilder.CreateIndex(
                name: "IX_UpdatedBy_Id11",
                table: "PatientStatusHistory",
                column: "UpdatedBy_Id");

            migrationBuilder.CreateIndex(
                name: "IX_ScriptFileName",
                table: "PostDeployment",
                column: "ScriptFileName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CreatedBy_Id12",
                table: "Pregnancy",
                column: "CreatedBy_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Patient_Id11",
                table: "Pregnancy",
                column: "Patient_Id");

            migrationBuilder.CreateIndex(
                name: "IX_UpdatedBy_Id12",
                table: "Pregnancy",
                column: "UpdatedBy_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Priority_Description",
                table: "Priority",
                column: "Description",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Concept_Id3",
                table: "Product",
                column: "Concept_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Product_ProductName",
                table: "Product",
                column: "ProductName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_User_Id",
                table: "RefreshToken",
                column: "User_Id");

            migrationBuilder.CreateIndex(
                name: "IX_CreatedBy_Id13",
                table: "ReportInstance",
                column: "CreatedBy_Id");

            migrationBuilder.CreateIndex(
                name: "IX_TerminologyMedDra_Id3",
                table: "ReportInstance",
                column: "TerminologyMedDra_Id");

            migrationBuilder.CreateIndex(
                name: "IX_UpdatedBy_Id13",
                table: "ReportInstance",
                column: "UpdatedBy_Id");

            migrationBuilder.CreateIndex(
                name: "IX_WorkFlow_Id1",
                table: "ReportInstance",
                column: "WorkFlow_Id");

            migrationBuilder.CreateIndex(
                name: "IX_ReportInstance_Id1",
                table: "ReportInstanceMedication",
                column: "ReportInstance_Id");

            migrationBuilder.CreateIndex(
                name: "IX_RiskFactor_FactorName",
                table: "RiskFactor",
                column: "FactorName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RiskFactor_Id",
                table: "RiskFactorOption",
                column: "RiskFactor_Id");

            migrationBuilder.CreateIndex(
                name: "IX_RiskFactorOption_OptionName",
                table: "RiskFactorOption",
                column: "OptionName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Role_Key",
                table: "Role",
                column: "Key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Role_Name",
                table: "Role",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SelectionDataItem_AttributeKey",
                table: "SelectionDataItem",
                column: "AttributeKey",
                unique: true,
                filter: "[AttributeKey] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_CreatedBy_Id14",
                table: "SiteContactDetail",
                column: "CreatedBy_Id");

            migrationBuilder.CreateIndex(
                name: "IX_UpdatedBy_Id14",
                table: "SiteContactDetail",
                column: "UpdatedBy_Id");

            migrationBuilder.CreateIndex(
                name: "IX_CreatedBy_Id15",
                table: "SystemLog",
                column: "CreatedBy_Id");

            migrationBuilder.CreateIndex(
                name: "IX_UpdatedBy_Id15",
                table: "SystemLog",
                column: "UpdatedBy_Id");

            migrationBuilder.CreateIndex(
                name: "IX_TerminologyIcd10_Description",
                table: "TerminologyIcd10",
                column: "Description",
                unique: true,
                filter: "[Description] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_TerminologyIcd10_Name",
                table: "TerminologyIcd10",
                column: "Name",
                unique: true,
                filter: "[Name] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Parent_Id1",
                table: "TerminologyMedDra",
                column: "Parent_Id");

            migrationBuilder.CreateIndex(
                name: "IX_TerminologyMedDra_MedDraCode",
                table: "TerminologyMedDra",
                column: "MedDraCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TerminologyMedDra_MedDraTerm",
                table: "TerminologyMedDra",
                column: "MedDraTerm",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TreatmentOutcome_Description",
                table: "TreatmentOutcome",
                column: "Description",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_User_UserName",
                table: "User",
                column: "UserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Facility_Id",
                table: "UserFacility",
                column: "Facility_Id");

            migrationBuilder.CreateIndex(
                name: "IX_User_Id1",
                table: "UserFacility",
                column: "User_Id");

            migrationBuilder.CreateIndex(
                name: "IX_UserFacility_User_Id_Facility_Id",
                table: "UserFacility",
                columns: new[] { "User_Id", "Facility_Id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Role_Id",
                table: "UserRole",
                column: "Role_Id");

            migrationBuilder.CreateIndex(
                name: "IX_User_Id2",
                table: "UserRole",
                column: "User_Id");

            migrationBuilder.CreateIndex(
                name: "IX_UserRole_User_Id_Role_Id",
                table: "UserRole",
                columns: new[] { "User_Id", "Role_Id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WorkFlow_Description",
                table: "WorkFlow",
                column: "Description",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Dataset_Id3",
                table: "WorkPlan",
                column: "Dataset_Id");

            migrationBuilder.CreateIndex(
                name: "IX_WorkPlan_Description",
                table: "WorkPlan",
                column: "Description",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CareEvent_Id",
                table: "WorkPlanCareEvent",
                column: "CareEvent_Id");

            migrationBuilder.CreateIndex(
                name: "IX_WorkPlan_Id1",
                table: "WorkPlanCareEvent",
                column: "WorkPlan_Id");

            migrationBuilder.CreateIndex(
                name: "IX_WorkPlanCareEvent_WorkPlan_Id_CareEvent_Id",
                table: "WorkPlanCareEvent",
                columns: new[] { "WorkPlan_Id", "CareEvent_Id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DatasetCategory_Id2",
                table: "WorkPlanCareEventDatasetCategory",
                column: "DatasetCategory_Id");

            migrationBuilder.CreateIndex(
                name: "IX_WorkPlanCareEvent_Id",
                table: "WorkPlanCareEventDatasetCategory",
                column: "WorkPlanCareEvent_Id");

            migrationBuilder.CreateIndex(
                name: "IX_WorkPlanCareEventDatasetCategory_WorkPlanCareEvent_Id_DatasetCategory_Id",
                table: "WorkPlanCareEventDatasetCategory",
                columns: new[] { "WorkPlanCareEvent_Id", "DatasetCategory_Id" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_dbo.WorkPlanCareEvent_dbo.WorkPlan_WorkPlan_Id",
                table: "WorkPlanCareEvent",
                column: "WorkPlan_Id",
                principalTable: "WorkPlan",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_dbo.EncounterTypeWorkPlan_dbo.WorkPlan_WorkPlan_Id",
                table: "EncounterTypeWorkPlan",
                column: "WorkPlan_Id",
                principalTable: "WorkPlan",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_dbo.Dataset_dbo.User_CreatedBy_Id",
                table: "Dataset");

            migrationBuilder.DropForeignKey(
                name: "FK_dbo.Dataset_dbo.User_UpdatedBy_Id",
                table: "Dataset");

            migrationBuilder.DropForeignKey(
                name: "FK_dbo.DatasetXml_dbo.User_CreatedBy_Id",
                table: "DatasetXml");

            migrationBuilder.DropForeignKey(
                name: "FK_dbo.DatasetXml_dbo.User_UpdatedBy_Id",
                table: "DatasetXml");

            migrationBuilder.DropForeignKey(
                name: "FK_dbo.CohortGroup_dbo.Condition_Condition_Id",
                table: "CohortGroup");

            migrationBuilder.DropForeignKey(
                name: "FK_dbo.EncounterTypeWorkPlan_dbo.CohortGroup_CohortGroup_Id",
                table: "EncounterTypeWorkPlan");

            migrationBuilder.DropForeignKey(
                name: "FK_dbo.Dataset_dbo.ContextType_ContextType_Id",
                table: "Dataset");

            migrationBuilder.DropForeignKey(
                name: "FK_dbo.Dataset_dbo.DatasetXml_DatasetXml_Id",
                table: "Dataset");

            migrationBuilder.DropForeignKey(
                name: "FK_dbo.Dataset_dbo.EncounterTypeWorkPlan_EncounterTypeWorkPlan_Id",
                table: "Dataset");

            migrationBuilder.DropTable(
                name: "Appointment");

            migrationBuilder.DropTable(
                name: "Attachment");

            migrationBuilder.DropTable(
                name: "AuditLog");

            migrationBuilder.DropTable(
                name: "CohortGroupEnrolment");

            migrationBuilder.DropTable(
                name: "ConceptIngredient");

            migrationBuilder.DropTable(
                name: "ConditionLabTest");

            migrationBuilder.DropTable(
                name: "ConditionMedDra");

            migrationBuilder.DropTable(
                name: "ConditionMedication");

            migrationBuilder.DropTable(
                name: "Config");

            migrationBuilder.DropTable(
                name: "CustomAttributeConfiguration");

            migrationBuilder.DropTable(
                name: "DatasetCategoryCondition");

            migrationBuilder.DropTable(
                name: "DatasetCategoryElementCondition");

            migrationBuilder.DropTable(
                name: "DatasetInstanceSubValue");

            migrationBuilder.DropTable(
                name: "DatasetMappingValue");

            migrationBuilder.DropTable(
                name: "DatasetRule");

            migrationBuilder.DropTable(
                name: "DatasetXmlAttribute");

            migrationBuilder.DropTable(
                name: "FieldValue");

            migrationBuilder.DropTable(
                name: "Holiday");

            migrationBuilder.DropTable(
                name: "LabResult");

            migrationBuilder.DropTable(
                name: "MedDRAGrading");

            migrationBuilder.DropTable(
                name: "MetaColumn");

            migrationBuilder.DropTable(
                name: "MetaDependency");

            migrationBuilder.DropTable(
                name: "MetaForm");

            migrationBuilder.DropTable(
                name: "MetaReport");

            migrationBuilder.DropTable(
                name: "MetaWidget");

            migrationBuilder.DropTable(
                name: "PatientClinicalEvent");

            migrationBuilder.DropTable(
                name: "PatientCondition");

            migrationBuilder.DropTable(
                name: "PatientFacility");

            migrationBuilder.DropTable(
                name: "PatientLabTest");

            migrationBuilder.DropTable(
                name: "PatientLanguage");

            migrationBuilder.DropTable(
                name: "PatientMedication");

            migrationBuilder.DropTable(
                name: "PatientStatusHistory");

            migrationBuilder.DropTable(
                name: "PostDeployment");

            migrationBuilder.DropTable(
                name: "RefreshToken");

            migrationBuilder.DropTable(
                name: "ReportInstanceMedication");

            migrationBuilder.DropTable(
                name: "RiskFactorOption");

            migrationBuilder.DropTable(
                name: "SiteContactDetail");

            migrationBuilder.DropTable(
                name: "SystemLog");

            migrationBuilder.DropTable(
                name: "TerminologyIcd10");

            migrationBuilder.DropTable(
                name: "UserFacility");

            migrationBuilder.DropTable(
                name: "UserRole");

            migrationBuilder.DropTable(
                name: "WorkPlanCareEventDatasetCategory");

            migrationBuilder.DropTable(
                name: "ActivityExecutionStatusEvent");

            migrationBuilder.DropTable(
                name: "AttachmentType");

            migrationBuilder.DropTable(
                name: "DatasetInstanceValue");

            migrationBuilder.DropTable(
                name: "DatasetMappingSub");

            migrationBuilder.DropTable(
                name: "DatasetXmlNode");

            migrationBuilder.DropTable(
                name: "MedDRAScale");

            migrationBuilder.DropTable(
                name: "MetaColumnType");

            migrationBuilder.DropTable(
                name: "MetaTable");

            migrationBuilder.DropTable(
                name: "MetaPage");

            migrationBuilder.DropTable(
                name: "MetaWidgetType");

            migrationBuilder.DropTable(
                name: "Encounter");

            migrationBuilder.DropTable(
                name: "Outcome");

            migrationBuilder.DropTable(
                name: "TreatmentOutcome");

            migrationBuilder.DropTable(
                name: "LabTest");

            migrationBuilder.DropTable(
                name: "LabTestUnit");

            migrationBuilder.DropTable(
                name: "Language");

            migrationBuilder.DropTable(
                name: "Product");

            migrationBuilder.DropTable(
                name: "PatientStatus");

            migrationBuilder.DropTable(
                name: "RiskFactor");

            migrationBuilder.DropTable(
                name: "Facility");

            migrationBuilder.DropTable(
                name: "Role");

            migrationBuilder.DropTable(
                name: "WorkPlanCareEvent");

            migrationBuilder.DropTable(
                name: "ActivityInstance");

            migrationBuilder.DropTable(
                name: "DatasetInstance");

            migrationBuilder.DropTable(
                name: "DatasetMapping");

            migrationBuilder.DropTable(
                name: "DatasetElementSub");

            migrationBuilder.DropTable(
                name: "SelectionDataItem");

            migrationBuilder.DropTable(
                name: "MetaTableType");

            migrationBuilder.DropTable(
                name: "Pregnancy");

            migrationBuilder.DropTable(
                name: "Priority");

            migrationBuilder.DropTable(
                name: "Concept");

            migrationBuilder.DropTable(
                name: "FacilityType");

            migrationBuilder.DropTable(
                name: "OrgUnit");

            migrationBuilder.DropTable(
                name: "CareEvent");

            migrationBuilder.DropTable(
                name: "ActivityExecutionStatus");

            migrationBuilder.DropTable(
                name: "ReportInstance");

            migrationBuilder.DropTable(
                name: "DatasetCategoryElement");

            migrationBuilder.DropTable(
                name: "Patient");

            migrationBuilder.DropTable(
                name: "MedicationForm");

            migrationBuilder.DropTable(
                name: "OrgUnitTypes");

            migrationBuilder.DropTable(
                name: "Activity");

            migrationBuilder.DropTable(
                name: "TerminologyMedDra");

            migrationBuilder.DropTable(
                name: "DatasetCategory");

            migrationBuilder.DropTable(
                name: "DatasetElement");

            migrationBuilder.DropTable(
                name: "WorkFlow");

            migrationBuilder.DropTable(
                name: "DatasetElementType");

            migrationBuilder.DropTable(
                name: "Field");

            migrationBuilder.DropTable(
                name: "FieldType");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropTable(
                name: "Condition");

            migrationBuilder.DropTable(
                name: "CohortGroup");

            migrationBuilder.DropTable(
                name: "ContextTypes");

            migrationBuilder.DropTable(
                name: "DatasetXml");

            migrationBuilder.DropTable(
                name: "EncounterTypeWorkPlan");

            migrationBuilder.DropTable(
                name: "EncounterType");

            migrationBuilder.DropTable(
                name: "WorkPlan");

            migrationBuilder.DropTable(
                name: "Dataset");
        }
    }
}
