namespace PVIMS.Entities.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddConceptToConditionGroup : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.ConditionMedication", new[] { "Condition_Id" });
            AddColumn("dbo.ConditionMedication", "Concept_Id", c => c.Int());
            AddColumn("dbo.ConditionMedication", "Product_Id", c => c.Int());
            AlterColumn("dbo.ConditionMedication", "Condition_Id", c => c.Int(nullable: false));
            CreateIndex("dbo.ConditionMedication", "Concept_Id");
            CreateIndex("dbo.ConditionMedication", "Condition_Id");
            CreateIndex("dbo.ConditionMedication", "Product_Id");
            AddForeignKey("dbo.ConditionMedication", "Concept_Id", "dbo.Concept", "Id");
            AddForeignKey("dbo.ConditionMedication", "Product_Id", "dbo.Product", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ConditionMedication", "Product_Id", "dbo.Product");
            DropForeignKey("dbo.ConditionMedication", "Concept_Id", "dbo.Concept");
            DropIndex("dbo.ConditionMedication", new[] { "Product_Id" });
            DropIndex("dbo.ConditionMedication", new[] { "Condition_Id" });
            DropIndex("dbo.ConditionMedication", new[] { "Concept_Id" });
            AlterColumn("dbo.ConditionMedication", "Condition_Id", c => c.Int());
            DropColumn("dbo.ConditionMedication", "Product_Id");
            DropColumn("dbo.ConditionMedication", "Concept_Id");
            CreateIndex("dbo.ConditionMedication", "Condition_Id");
        }
    }
}
