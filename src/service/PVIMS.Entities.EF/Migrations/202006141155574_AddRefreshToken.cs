namespace PVIMS.Entities.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddRefreshToken : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.RefreshToken",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Token = c.String(),
                        Expires = c.DateTime(nullable: false),
                        UserId = c.Guid(nullable: false),
                        RemoteIpAddress = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.RefreshToken");
        }
    }
}
