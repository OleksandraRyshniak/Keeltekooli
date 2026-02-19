using Keeltekooli.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Keeltekooli.Controllers
{

    public class OpetajasController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Opetajas

        public ActionResult Index()
        {
            var opetajad = db.Opetaja
        .Select(o => new OpetajaViewModel
        {
            Id = o.Id,
            Nimi = o.Nimi,
            Kvalifikatsioon = o.Kvalifikatsioon,
            FotoPath = o.FotoPath,
            Email = o.User.Email,
        })
        .ToList();

            return View(opetajad);
        }

        // GET: Opetajas/Details/5
        public ActionResult Details(int? id)
        {
    if (id == null)
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

    var opetaja = db.Opetaja
        .Where(o => o.Id == id)
        .Select(o => new OpetajaViewModel
        {
            Id = o.Id,
            Nimi = o.Nimi,
            Kvalifikatsioon = o.Kvalifikatsioon,
            FotoPath = o.FotoPath,
            Email = o.User.Email
        })
        .FirstOrDefault();

    if (opetaja == null)
        return HttpNotFound();

    return View(opetaja);
        }

        // GET: Opetajas/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Opetajas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(OpetajaViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var userStore = new UserStore<ApplicationUser>(db);
            var userManager = new UserManager<ApplicationUser>(userStore);

            if (userManager.FindByEmail(model.Email) != null)
            {
                ModelState.AddModelError("", "Kasutaja selle e-posti aadressiga juba olemas!");
                return View(model);
            }

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email
            };
            var result = userManager.Create(user, model.Password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error);
                return View(model);
            }

            userManager.AddToRole(user.Id, "Opetaja");

            var opetaja = new Opetaja
            {
                Nimi = model.Nimi,
                Kvalifikatsioon = model.Kvalifikatsioon,
                FotoPath = model.FotoPath,
                ApplicationUserId = user.Id
            };

            db.Opetaja.Add(opetaja);
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        // GET: Opetajas/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var opetaja = db.Opetaja
                .Where(o => o.Id == id)
                .Select(o => new OpetajaViewModel
                {
                    Id = o.Id,
                    Nimi = o.Nimi,
                    Kvalifikatsioon = o.Kvalifikatsioon,
                    FotoPath = o.FotoPath,
                    Email = o.User.Email
                })
                .FirstOrDefault();

            if (opetaja == null)
                return HttpNotFound();

            return View(opetaja);
        }

        // POST: Opetajas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(OpetajaViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // находим Opetaja и связанного пользователя Identity
            var opetaja = db.Opetaja.Include(o => o.User).FirstOrDefault(o => o.Id == model.Id);
            if (opetaja == null)
                return HttpNotFound();

            // Обновляем поля Opetaja
            opetaja.Nimi = model.Nimi;
            opetaja.Kvalifikatsioon = model.Kvalifikatsioon;
            opetaja.FotoPath = model.FotoPath;

            // Обновляем Email пользователя
            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            opetaja.User.Email = model.Email;
            opetaja.User.UserName = model.Email;

            if (!string.IsNullOrEmpty(model.Password))
            {
                // Сброс пароля
                var token = userManager.GeneratePasswordResetToken(opetaja.User.Id);
                userManager.ResetPassword(opetaja.User.Id, token, model.Password);
            }

            db.SaveChanges();

            return RedirectToAction("Index");
        }


        // GET: Opetajas/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Opetaja opetaja = db.Opetaja.Find(id);
            if (opetaja == null)
            {
                return HttpNotFound();
            }
            return View(opetaja);
        }

        // POST: Opetajas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Opetaja opetaja = db.Opetaja.Find(id);
            db.Opetaja.Remove(opetaja);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public ActionResult MinuKoolitused()
        {
            string userId = User.Identity.GetUserId();

            // 1️⃣ Находим профиль учителя
            var opetaja = db.Opetaja
                .FirstOrDefault(o => o.ApplicationUserId == userId);

            if (opetaja == null)
                return HttpNotFound("Õpetaja profiili ei leitud");

            // 2️⃣ Загружаем ТОЛЬКО ЕГО курсы / группы
            var minuKoolitused = db.Koolitus
                .Include(k => k.Keelekursus)
                .Include(k => k.Registreerimised)
                .Where(k => k.OpetajaId == opetaja.Id)
                .ToList();

            return View(minuKoolitused);
        }
    }
}
