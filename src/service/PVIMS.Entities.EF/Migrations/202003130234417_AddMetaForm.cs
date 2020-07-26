namespace PVIMS.Entities.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddMetaForm : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.MetaForm",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        metaform_guid = c.Guid(nullable: false),
                        FormName = c.String(nullable: false, maxLength: 50),
                        MetaDefinition = c.String(),
                        IsSystem = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.MetaForm");
        }
    }
}
