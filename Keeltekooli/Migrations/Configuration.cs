using Keeltekooli.Models;
using Microsoft.AspNet.Identity;

namespace Keeltekooli.Migrations
{
    using Keeltekooli.Models;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<Keeltekooli.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(Keeltekooli.Models.ApplicationDbContext context)
        {
            var roleStore = new RoleStore<IdentityRole>(context);
            var roleManager = new RoleManager<IdentityRole>(roleStore);
            var userStore = new UserStore<ApplicationUser>(context);
            var userManager = new UserManager<ApplicationUser>(userStore);
            
            string[] roles = { "Admin", "Opetja", "Opilane" };

            foreach (var roleName in roles)
            {
                if (!roleManager.RoleExists(roleName))
                {
                    roleManager.Create(new IdentityRole(roleName));
                }
            }

            // admin
            string adminEmail = "alexaryshniak@gmail.com";
            string adminPassword = "Parol123!";

            var adminUser = userManager.FindByEmail(adminEmail);
            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail
                };

            userManager.Create(adminUser, adminPassword);
            }

            if (!userManager.IsInRole(adminUser.Id, "Admin"))
                {
                    userManager.AddToRole(adminUser.Id, "Admin");
                }

            //õpetaja
            string opetajaEmail = "teacher@keeltekool.ee";
            string opetajaPassword = "Parol123!";

            var opetajaUser = userManager.FindByEmail(opetajaEmail);
            if (opetajaUser == null)
            {
                opetajaUser = new ApplicationUser
                {
                    UserName = opetajaEmail,
                    Email = opetajaEmail
                };

                userManager.Create(opetajaUser, opetajaPassword);
            }

            if (!userManager.IsInRole(opetajaUser.Id, "Opetaja"))
            {
                userManager.AddToRole(opetajaUser.Id, "Opetaja");
            }
            if (!context.Opetaja.Any(o => o.ApplicationUserId == opetajaUser.Id))
            {
                context.Opetaja.Add(new Opetaja
                {
                    Nimi = "Mari Maasikas",
                    Kvalifikatsioon = "Magister, Saksa keel",
                    ApplicationUserId = opetajaUser.Id,
                    FotoPath = "mari.jpg"
                });
            }

            //opilane
            string opilaneEmail = "student@keeltekool.ee";
            string opilanePassword = "Parol123!";

            var opilaneUser = userManager.FindByEmail(opilaneEmail);
            if (opilaneUser == null)
            {
                opilaneUser = new ApplicationUser
                {
                    UserName = opilaneEmail,
                    Email = opilaneEmail
                };

                userManager.Create(opilaneUser, opilanePassword);
            }

            if (!userManager.IsInRole(opilaneUser.Id, "Opilane"))
            {
                userManager.AddToRole(opilaneUser.Id, "Opilane");
            }

            context.SaveChanges();
        }     
    }
}
