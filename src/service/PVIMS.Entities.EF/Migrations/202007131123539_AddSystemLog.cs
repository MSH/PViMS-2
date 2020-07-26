namespace PVIMS.Entities.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddSystemLog : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SystemLog",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Sender = c.String(nullable: false),
                        EventType = c.String(nullable: false),
                        ExceptionCode = c.String(nullable: false),
                        ExceptionMessage = c.String(nullable: false),
                        ExceptionStackTrace = c.String(),
                        InnerExceptionMessage = c.String(),
                        InnerExceptionStackTrace = c.String(),
                        RemoteIpAddress = c.String(),
                        Created = c.DateTime(nullable: false),
                        LastUpdated = c.DateTime(),
                        CreatedBy_Id = c.Int(),
                        UpdatedBy_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.User", t => t.CreatedBy_Id)
                .ForeignKey("dbo.User", t => t.UpdatedBy_Id)
                .Index(t => t.CreatedBy_Id)
                .Index(t => t.UpdatedBy_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SystemLog", "UpdatedBy_Id", "dbo.User");
            DropForeignKey("dbo.SystemLog", "CreatedBy_Id", "dbo.User");
            DropIndex("dbo.SystemLog", new[] { "UpdatedBy_Id" });
            DropIndex("dbo.SystemLog", new[] { "CreatedBy_Id" });
            DropTable("dbo.SystemLog");
        }
    }
}
