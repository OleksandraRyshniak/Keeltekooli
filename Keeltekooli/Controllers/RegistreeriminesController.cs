using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
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

            if (koolitusId.HasValue)
            {
                // Проверяем, что курс реально существует
                var koolitus = db.Koolitus.Find(koolitusId.Value);
                if (koolitus == null)
                    return HttpNotFound();

                model.KoolitusId = koolitus.Id; // автоматически устанавливаем
            }

            ViewBag.KoolitusId = new SelectList(db.Koolitus, "Id", "Koolitus", model.KoolitusId);
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
                ViewBag.KoolitusId = new SelectList(db.Koolitus, "Id", "Id", model.KoolitusId);
                return View(model);
            }

            // Проверяем email пользователя
            var user = db.Users.FirstOrDefault(u => u.Email == model.Email);
            if (user == null)
            {
                ModelState.AddModelError("", "Kasutaja selle e-posti aadressiga juba olemas!");
                ViewBag.KoolitusId = new SelectList(db.Koolitus, "Id", "Id", model.KoolitusId);
                return View(model);
            }

            // Проверяем, что выбранный курс существует
            if (model.KoolitusId == 0 || db.Koolitus.Find(model.KoolitusId) == null)
            {
                ModelState.AddModelError("", "Valitud grupp ei eksisteeri!");
                ViewBag.KoolitusId = new SelectList(db.Koolitus, "Id", "Nimi", model.KoolitusId);
                return View(model);
            }

            // Создаём сущность и сохраняем
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
