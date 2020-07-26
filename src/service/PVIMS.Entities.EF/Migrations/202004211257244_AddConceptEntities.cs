namespace PVIMS.Entities.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddConceptEntities : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ConceptIngredient",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Ingredient = c.String(nullable: false, maxLength: 200),
                        Strength = c.String(nullable: false, maxLength: 50),
                        Active = c.Boolean(nullable: false),
                        Concept_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Concept", t => t.Concept_Id)
                .Index(t => t.Concept_Id);
            
            CreateTable(
                "dbo.Concept",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ConceptName = c.String(nullable: false, maxLength: 1000),
                        Active = c.Boolean(nullable: false),
                        MedicationForm_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.MedicationForm", t => t.MedicationForm_Id)
                .Index(t => t.MedicationForm_Id);
            
            CreateTable(
                "dbo.Product",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProductName = c.String(nullable: false, maxLength: 200),
                        Manufacturer = c.String(nullable: false, maxLength: 200),
                        Description = c.String(maxLength: 1000),
                        Active = c.Boolean(nullable: false),
                        Concept_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Concept", t => t.Concept_Id)
                .Index(t => t.Concept_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ConceptIngredient", "Concept_Id", "dbo.Concept");
            DropForeignKey("dbo.Product", "Concept_Id", "dbo.Concept");
            DropForeignKey("dbo.Concept", "MedicationForm_Id", "dbo.MedicationForm");
            DropIndex("dbo.Product", new[] { "Concept_Id" });
            DropIndex("dbo.Concept", new[] { "MedicationForm_Id" });
            DropIndex("dbo.ConceptIngredient", new[] { "Concept_Id" });
            DropTable("dbo.Product");
            DropTable("dbo.Concept");
            DropTable("dbo.ConceptIngredient");
        }
    }
}
