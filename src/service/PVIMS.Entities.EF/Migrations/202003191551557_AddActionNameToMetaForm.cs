namespace PVIMS.Entities.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddActionNameToMetaForm : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MetaForm", "ActionName", c => c.String(nullable: false, maxLength: 20));
        }
        
        public override void Down()
        {
            DropColumn("dbo.MetaForm", "ActionName");
        }
    }
}
