namespace PVIMS.Entities.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ActiveStatusForLabResultAndCondition : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Condition", "Active", c => c.Boolean(nullable: false));
            AddColumn("dbo.LabResult", "Active", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.LabResult", "Active");
            DropColumn("dbo.Condition", "Active");
        }
    }
}
