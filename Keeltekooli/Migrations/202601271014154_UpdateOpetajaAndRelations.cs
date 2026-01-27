namespace Keeltekooli.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateOpetajaAndRelations : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Opetajas", "ApplicationUserId", c => c.String(maxLength: 128));
            CreateIndex("dbo.Opetajas", "ApplicationUserId");
            AddForeignKey("dbo.Opetajas", "ApplicationUserId", "dbo.AspNetUsers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Opetajas", "ApplicationUserId", "dbo.AspNetUsers");
            DropIndex("dbo.Opetajas", new[] { "ApplicationUserId" });
            AlterColumn("dbo.Opetajas", "ApplicationUserId", c => c.String());
        }
    }
}
