namespace PVIMS.Entities.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddIdentityToUser : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.User", "IdentityId", c => c.Guid(nullable: false));
            AddColumn("dbo.User", "UserType", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.User", "UserType");
            DropColumn("dbo.User", "IdentityId");
        }
    }
}
