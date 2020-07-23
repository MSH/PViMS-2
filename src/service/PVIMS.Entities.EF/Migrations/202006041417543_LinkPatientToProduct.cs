namespace PVIMS.Entities.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LinkPatientToProduct : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.PatientMedication", new[] { "Patient_Id" });
            AddColumn("dbo.PatientMedication", "Concept_Id", c => c.Int());
            AddColumn("dbo.PatientMedication", "Product_Id", c => c.Int());
            AlterColumn("dbo.PatientMedication", "Patient_Id", c => c.Int(nullable: false));
            CreateIndex("dbo.PatientMedication", "Concept_Id");
            CreateIndex("dbo.PatientMedication", "Patient_Id");
            CreateIndex("dbo.PatientMedication", "Product_Id");
            AddForeignKey("dbo.PatientMedication", "Concept_Id", "dbo.Concept", "Id");
            AddForeignKey("dbo.PatientMedication", "Product_Id", "dbo.Product", "Id");
            DropColumn("dbo.PatientClinicalEvent", "ClinicalEventSource");
        }
        
        public override void Down()
        {
            AddColumn("dbo.PatientClinicalEvent", "ClinicalEventSource", c => c.String(maxLength: 200));
            DropForeignKey("dbo.PatientMedication", "Product_Id", "dbo.Product");
            DropForeignKey("dbo.PatientMedication", "Concept_Id", "dbo.Concept");
            DropIndex("dbo.PatientMedication", new[] { "Product_Id" });
            DropIndex("dbo.PatientMedication", new[] { "Patient_Id" });
            DropIndex("dbo.PatientMedication", new[] { "Concept_Id" });
            AlterColumn("dbo.PatientMedication", "Patient_Id", c => c.Int());
            DropColumn("dbo.PatientMedication", "Product_Id");
            DropColumn("dbo.PatientMedication", "Concept_Id");
            CreateIndex("dbo.PatientMedication", "Patient_Id");
        }
    }
}
