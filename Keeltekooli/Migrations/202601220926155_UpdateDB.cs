namespace Keeltekooli.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateDB : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Opetajas", "ApplicationUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Registreerimines", "ApplicationUserId", "dbo.AspNetUsers");
            DropIndex("dbo.Opetajas", new[] { "ApplicationUserId" });
            DropIndex("dbo.Registreerimines", new[] { "ApplicationUserId" });
            AlterColumn("dbo.Opetajas", "ApplicationUserId", c => c.String());
            AlterColumn("dbo.Registreerimines", "ApplicationUserId", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Registreerimines", "ApplicationUserId", c => c.String(maxLength: 128));
            AlterColumn("dbo.Opetajas", "ApplicationUserId", c => c.String(maxLength: 128));
            CreateIndex("dbo.Registreerimines", "ApplicationUserId");
            CreateIndex("dbo.Opetajas", "ApplicationUserId");
            AddForeignKey("dbo.Registreerimines", "ApplicationUserId", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.Opetajas", "ApplicationUserId", "dbo.AspNetUsers", "Id");
        }
    }
}
