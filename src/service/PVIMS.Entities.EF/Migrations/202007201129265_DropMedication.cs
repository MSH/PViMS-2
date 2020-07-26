namespace PVIMS.Entities.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DropMedication : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ConditionMedication", "Medication_Id", "dbo.Medication");
            DropForeignKey("dbo.Medication", "MedicationForm_Id", "dbo.MedicationForm");
            DropForeignKey("dbo.PatientClinicalEvent", "Medication_Id", "dbo.Medication");
            DropForeignKey("dbo.PatientMedication", "Medication_Id", "dbo.Medication");
            DropIndex("dbo.PatientClinicalEvent", new[] { "Medication_Id" });
            DropIndex("dbo.ConditionMedication", new[] { "Medication_Id" });
            DropIndex("dbo.Medication", new[] { "MedicationForm_Id" });
            DropIndex("dbo.PatientMedication", new[] { "Medication_Id" });
            DropColumn("dbo.PatientClinicalEvent", "Medication_Id");
            DropColumn("dbo.ConditionMedication", "Medication_Id");
            DropColumn("dbo.PatientMedication", "Medication_Id");
            DropTable("dbo.Medication");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Medication",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DrugName = c.String(nullable: false, maxLength: 100),
                        Active = c.Boolean(nullable: false),
                        PackSize = c.Int(nullable: false),
                        Strength = c.String(nullable: false, maxLength: 40),
                        CatalogNo = c.String(maxLength: 10),
                        MedicationForm_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.PatientMedication", "Medication_Id", c => c.Int());
            AddColumn("dbo.ConditionMedication", "Medication_Id", c => c.Int());
            AddColumn("dbo.PatientClinicalEvent", "Medication_Id", c => c.Int());
            CreateIndex("dbo.PatientMedication", "Medication_Id");
            CreateIndex("dbo.Medication", "MedicationForm_Id");
            CreateIndex("dbo.ConditionMedication", "Medication_Id");
            CreateIndex("dbo.PatientClinicalEvent", "Medication_Id");
            AddForeignKey("dbo.PatientMedication", "Medication_Id", "dbo.Medication", "Id");
            AddForeignKey("dbo.PatientClinicalEvent", "Medication_Id", "dbo.Medication", "Id");
            AddForeignKey("dbo.Medication", "MedicationForm_Id", "dbo.MedicationForm", "Id");
            AddForeignKey("dbo.ConditionMedication", "Medication_Id", "dbo.Medication", "Id");
        }
    }
}
