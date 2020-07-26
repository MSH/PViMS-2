namespace PVIMS.Entities.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RefreshTokenReference : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RefreshToken", "User_Id", c => c.Int());
            CreateIndex("dbo.RefreshToken", "User_Id");
            AddForeignKey("dbo.RefreshToken", "User_Id", "dbo.User", "Id");
            DropColumn("dbo.RefreshToken", "UserId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.RefreshToken", "UserId", c => c.Guid(nullable: false));
            DropForeignKey("dbo.RefreshToken", "User_Id", "dbo.User");
            DropIndex("dbo.RefreshToken", new[] { "User_Id" });
            DropColumn("dbo.RefreshToken", "User_Id");
        }
    }
}
