namespace PVIMS.Entities.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddPatientEntitySources : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PatientLabTest", "LabTestSource", c => c.String(maxLength: 200));
            AddColumn("dbo.PatientClinicalEvent", "ClinicalEventSource", c => c.String(maxLength: 200));
            AddColumn("dbo.PatientMedication", "MedicationSource", c => c.String(maxLength: 200));
            AddColumn("dbo.PatientCondition", "ConditionSource", c => c.String(maxLength: 200));
        }
        
        public override void Down()
        {
            DropColumn("dbo.PatientCondition", "ConditionSource");
            DropColumn("dbo.PatientMedication", "MedicationSource");
            DropColumn("dbo.PatientClinicalEvent", "ClinicalEventSource");
            DropColumn("dbo.PatientLabTest", "LabTestSource");
        }
    }
}
