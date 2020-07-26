namespace PVIMS.Entities.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DisableAutoGeneratedGuidForPatientInformationEntities : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.PatientClinicalEvent", "PatientClinicalEventGuid", c => c.Guid(nullable: false));
            AlterColumn("dbo.PatientCondition", "PatientConditionGuid", c => c.Guid(nullable: false));
            AlterColumn("dbo.PatientMedication", "PatientMedicationGuid", c => c.Guid(nullable: false));
            AlterColumn("dbo.PatientLabTest", "PatientLabTestGuid", c => c.Guid(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.PatientLabTest", "PatientLabTestGuid", c => c.Guid(nullable: false));
            AlterColumn("dbo.PatientMedication", "PatientMedicationGuid", c => c.Guid(nullable: false));
            AlterColumn("dbo.PatientCondition", "PatientConditionGuid", c => c.Guid(nullable: false));
            AlterColumn("dbo.PatientClinicalEvent", "PatientClinicalEventGuid", c => c.Guid(nullable: false));
        }
    }
}
