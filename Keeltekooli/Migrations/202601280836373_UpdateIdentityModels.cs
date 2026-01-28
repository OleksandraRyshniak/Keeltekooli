namespace Keeltekooli.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateIdentityModels : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Registreerimines", "ApplicationUserId", c => c.String(maxLength: 128));
            CreateIndex("dbo.Registreerimines", "ApplicationUserId");
            AddForeignKey("dbo.Registreerimines", "ApplicationUserId", "dbo.AspNetUsers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Registreerimines", "ApplicationUserId", "dbo.AspNetUsers");
            DropIndex("dbo.Registreerimines", new[] { "ApplicationUserId" });
            AlterColumn("dbo.Registreerimines", "ApplicationUserId", c => c.String());
        }
    }
}
