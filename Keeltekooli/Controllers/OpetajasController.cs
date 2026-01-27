using Keeltekooli.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
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
    [Authorize(Roles = "Admin")]
    public class OpetajasController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Opetajas

        public ActionResult Index()
        {
            return View(db.Opetaja.ToList());
        }

        // GET: Opetajas/Details/5
        public ActionResult Details(int? id)
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

            // Проверка, есть ли уже такой email
            if (userManager.FindByEmail(model.Email) != null)
            {
                ModelState.AddModelError("", "Пользователь с таким email уже существует!");
                return View(model);
            }

            // 1️⃣ Создаём пользователя Identity
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

            // 2️⃣ Добавляем роль "Opetaja"
            userManager.AddToRole(user.Id, "Opetaja");

            // 3️⃣ Создаём профиль Opetaja
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

        // POST: Opetajas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Nimi,Kvalifikatsioon,FotoPath,ApplicationUserId")] Opetaja opetaja)
        {
            if (ModelState.IsValid)
            {
                db.Entry(opetaja).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(opetaja);
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

        public ActionResult minukoolitus_opetaja()
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

        public ActionResult Details(int id)
        {
            string userId = User.Identity.GetUserId();

            var koolitus = db.Koolitus
                .Include(k => k.Keelekursus)
                .Include(k => k.Registreerimised.Select(r => r.ApplicationUser))
                .FirstOrDefault(k => k.Id == id &&
                                     k.Opetaja.ApplicationUserId == userId);

            if (koolitus == null)
                return new HttpUnauthorizedResult();

            return View(koolitus);
        }
    }
}
