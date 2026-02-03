using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Keeltekooli.Models;
using Microsoft.AspNet.Identity;

namespace Keeltekooli.Controllers
{

    public class KoolitusController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Koolitus
        [AllowAnonymous]
        public ActionResult Index(int? keelekursusId)
        {
            var koolitus = db.Koolitus
                .Include(k => k.Keelekursus)
                .Include(k => k.Opetaja);

            if (keelekursusId != null)
            {
                koolitus = koolitus
                    .Where(k => k.KeelekursusId == keelekursusId);

                var keelekursus = db.Keelekursus
                    .FirstOrDefault(k => k.Id == keelekursusId);

                if (keelekursus != null)
                {
                   ViewBag.p = keelekursus.Nimetus +" Koolitused";
                }
            }
            else
            {
                ViewBag.p = "Koolitused";
            }

            return View(koolitus.ToList());
        }

        // GET: Koolitus/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Koolitus koolitus = db.Koolitus.Find(id);
            if (koolitus == null)
            {
                return HttpNotFound();
            }
            return View(koolitus);
        }

        // GET: Koolitus/Create
        public ActionResult Create()
        {
            ViewBag.KeelekursusId = new SelectList(db.Keelekursus, "Id", "Nimetus");
            ViewBag.OpetajaId = new SelectList(db.Opetaja, "Id", "Nimi");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,KeelekursusId,OpetajaId,AlgusKuupaev,LoppKuupaev,Hind,MaxOsalejaid")] Koolitus koolitus)
        {
            if (ModelState.IsValid)
            {
                db.Koolitus.Add(koolitus);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.KeelekursusId = new SelectList(db.Keelekursus, "Id", "Nimetus", koolitus.KeelekursusId);
            ViewBag.OpetajaId = new SelectList(db.Opetaja, "Id", "Nimi", koolitus.OpetajaId);
            return View(koolitus);
        }

        // GET: Koolitus/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Koolitus koolitus = db.Koolitus.Find(id);
            if (koolitus == null)
            {
                return HttpNotFound();
            }
            ViewBag.KeelekursusId = new SelectList(db.Keelekursus, "Id", "Nimetus", koolitus.KeelekursusId);
            ViewBag.OpetajaId = new SelectList(db.Opetaja, "Id", "Nimi", koolitus.OpetajaId);
            return View(koolitus);
        }

        // POST: Koolitus/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,KeelekursusId,OpetajaId,AlgusKuupaev,LoppKuupaev,Hind,MaxOsalejaid")] Koolitus koolitus)
        {
            if (ModelState.IsValid)
            {
                db.Entry(koolitus).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.KeelekursusId = new SelectList(db.Keelekursus, "Id", "Nimetus", koolitus.KeelekursusId);
            ViewBag.OpetajaId = new SelectList(db.Opetaja, "Id", "Nimi", koolitus.OpetajaId);
            return View(koolitus);
        }

        // GET: Koolitus/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Koolitus koolitus = db.Koolitus.Find(id);
            if (koolitus == null)
            {
                return HttpNotFound();
            }
            return View(koolitus);
        }

        // POST: Koolitus/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Koolitus koolitus = db.Koolitus.Find(id);
            db.Koolitus.Remove(koolitus);
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
