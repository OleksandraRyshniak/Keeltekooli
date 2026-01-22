namespace Keeltekooli.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitKoolModels : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Keelekursus",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Nimetus = c.String(nullable: false),
                        Keel = c.String(nullable: false),
                        Tase = c.String(nullable: false),
                        Kirjeldus = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Koolitus",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        KeelekursusId = c.Int(nullable: false),
                        OpetajaId = c.Int(nullable: false),
                        AlgusKuupaev = c.DateTime(nullable: false),
                        LoppKuupaev = c.DateTime(nullable: false),
                        Hind = c.Single(nullable: false),
                        MaxOsalejaid = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Keelekursus", t => t.KeelekursusId, cascadeDelete: true)
                .ForeignKey("dbo.Opetajas", t => t.OpetajaId, cascadeDelete: true)
                .Index(t => t.KeelekursusId)
                .Index(t => t.OpetajaId);
            
            CreateTable(
                "dbo.Opetajas",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Nimi = c.String(nullable: false),
                        Kvalifikatsioon = c.String(),
                        FotoPath = c.String(),
                        ApplicationUserId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUserId)
                .Index(t => t.ApplicationUserId);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.Registreerimines",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        KoolitusId = c.Int(nullable: false),
                        ApplicationUserId = c.String(maxLength: 128),
                        Staatus = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Koolitus", t => t.KoolitusId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUserId)
                .Index(t => t.KoolitusId)
                .Index(t => t.ApplicationUserId);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.Registreerimines", "ApplicationUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Registreerimines", "KoolitusId", "dbo.Koolitus");
            DropForeignKey("dbo.Koolitus", "OpetajaId", "dbo.Opetajas");
            DropForeignKey("dbo.Opetajas", "ApplicationUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Koolitus", "KeelekursusId", "dbo.Keelekursus");
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.Registreerimines", new[] { "ApplicationUserId" });
            DropIndex("dbo.Registreerimines", new[] { "KoolitusId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.Opetajas", new[] { "ApplicationUserId" });
            DropIndex("dbo.Koolitus", new[] { "OpetajaId" });
            DropIndex("dbo.Koolitus", new[] { "KeelekursusId" });
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.Registreerimines");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.Opetajas");
            DropTable("dbo.Koolitus");
            DropTable("dbo.Keelekursus");
        }
    }
}
