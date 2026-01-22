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
        public ActionResult Create()
        {
            ViewBag.KoolitusId = new SelectList(db.Koolitus, "Id", "Id");
            return View();
        }

        // POST: Registreerimines/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,KoolitusId,ApplicationUserId,Staatus")] Registreerimine registreerimine)
        {
            if (ModelState.IsValid)
            {
                db.Registreerimine.Add(registreerimine);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.KoolitusId = new SelectList(db.Koolitus, "Id", "Id", registreerimine.KoolitusId);
            return View(registreerimine);
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
