using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Keeltekooli.Models
{
    public class RegistreeriminesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Registreerimines
        public ActionResult Index()
        {
            var registreerimine = db.Registreerimine.Include(r => r.Koolitus);
            return View(registreerimine.ToList());
        }

        // GET: Registreerimines/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Registreerimine registreerimine = db.Registreerimine.Find(id);
            if (registreerimine == null)
            {
                return HttpNotFound();
            }
            return View(registreerimine);
        }

        // GET: Registreerimines/Create
        public ActionResult Create(int? koolitusId)
        {
            var model = new RegistreerimineViewModel();

            // Находим курс по ID
            var koolitus = db.Koolitus
                .Include(k => k.Keelekursus)
                .Include(k => k.Opetaja)
                .FirstOrDefault(k => k.Id == koolitusId);

            var koolitus1 = db.Koolitus.Find(koolitusId);

            if (koolitus == null)
                return HttpNotFound();

            // Устанавливаем курс в модель
            model.KoolitusId = koolitus.Id;

            // Передаём курс в View через ViewBag для отображения заголовка
            ViewBag.Koolitus = koolitus;

            return View(model);
        }



        // POST: Registreerimines/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(RegistreerimineViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.KoolitusId = new SelectList(db.Koolitus, "Id", "Nimi", model.KoolitusId);
                return View(model);
            }

            // Проверяем, что курс существует
            var koolitus = db.Koolitus.Find(model.KoolitusId);
            if (koolitus == null)
            {
                ModelState.AddModelError("", "Valitud grupp ei eksisteeri!");
                ViewBag.KoolitusId = new SelectList(db.Koolitus, "Id", "Nimi", model.KoolitusId);
                return View(model);
            }

            // Проверяем пользователя
            var user = db.Users.SingleOrDefault(u => u.Email == model.Email);

            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email
                };

                db.Users.Add(user);
                db.SaveChanges();
            }

            // *** Проверяем, зарегистрирован ли пользователь уже на этот курс ***
            bool alreadyRegistered = db.Registreerimine.Any(r =>
                r.KoolitusId == model.KoolitusId &&
                r.ApplicationUserId == user.Id);

            if (alreadyRegistered)
            {
                ModelState.AddModelError("", "Sa oled juba registreeritud sellele kursusele.");
                ViewBag.KoolitusId = new SelectList(db.Koolitus, "Id", "Nimi", model.KoolitusId);
                return View(model);
            }

            // Создаём регистрацию
            var registreerimine = new Registreerimine
            {
                KoolitusId = model.KoolitusId,
                ApplicationUserId = user.Id,
                Staatus = "Vaatamisel"
            };

            db.Registreerimine.Add(registreerimine);
            db.SaveChanges();

            return RedirectToAction("Index");
        }



        // GET: Registreerimines/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Registreerimine registreerimine = db.Registreerimine.Find(id);
            if (registreerimine == null)
            {
                return HttpNotFound();
            }
            ViewBag.KoolitusId = new SelectList(db.Koolitus, "Id", "Id", registreerimine.KoolitusId);
            return View(registreerimine);
        }

        // POST: Registreerimines/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,KoolitusId,ApplicationUserId,Staatus")] Registreerimine registreerimine)
        {
            if (ModelState.IsValid)
            {
                db.Entry(registreerimine).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.KoolitusId = new SelectList(db.Koolitus, "Id", "Id", registreerimine.KoolitusId);
            return View(registreerimine);
        }

        // GET: Registreerimines/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Registreerimine registreerimine = db.Registreerimine.Find(id);
            if (registreerimine == null)
            {
                return HttpNotFound();
            }
            return View(registreerimine);
        }

        // POST: Registreerimines/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Registreerimine registreerimine = db.Registreerimine.Find(id);
            db.Registreerimine.Remove(registreerimine);
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
    }
}
