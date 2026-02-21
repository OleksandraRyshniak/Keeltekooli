using Keeltekooli.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace Keeltekooli.Controllers
{

    public class KoolitusController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Koolitus
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
                if (koolitus.LoppKuupaev < koolitus.AlgusKuupaev)
    {
        ModelState.AddModelError("", "Lõppkuupäev ei tohi olla enne alguskuupäeva");
    }

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

        public ActionResult Minukool()
        {
            string userId = User.Identity.GetUserId();

            var registreeringud = db.Registreerimine
                .Where(r => r.ApplicationUserId == userId)
                .Select(r => r.KoolitusId)
                .ToList();

            var minuKoolitused = db.Koolitus
                .Include(k => k.Keelekursus)
                .Include(k => k.Opetaja)
                .Where(k => registreeringud.Contains(k.Id))
                .ToList();

            return View(minuKoolitused);
        }

    public ActionResult Registreeru(int? koolitusId)
{
    if (koolitusId == null || koolitusId.Value <= 0)
    {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
    }

    var userId = User.Identity.GetUserId();
    var koolitus = db.Koolitus.Find(koolitusId.Value);
    if (koolitus == null)
    {
        return HttpNotFound();
    }

    // Проверка на повторную регистрацию
    var olemas = db.Registreerimine
        .FirstOrDefault(r => r.KoolitusId == koolitusId.Value
                          && r.ApplicationUserId == userId);

    if (olemas != null)
    {
        return RedirectToAction("Index", "Koolitus");
    }

    var reg = new Registreerimine
    {
        KoolitusId = koolitusId.Value,
        ApplicationUserId = userId,
        Staatus = "Ootel"
    };

    db.Registreerimine.Add(reg);
    db.SaveChanges();

    var user = db.Users.Find(userId);
    SaadaEmail(user?.Email, user?.UserName, koolitus.Keelekursus?.Nimetus ?? "kursus");

    return RedirectToAction("Tanan", "Home", new { id = reg.Id });
}

        private void SaadaEmail(string email, string nimi, string kursus)
        {
            try
            {
                WebMail.SmtpServer = "smtp.gmail.com";
                WebMail.SmtpPort = 587;
                WebMail.EnableSsl = true;
                WebMail.UserName = "YOUR_EMAIL@gmail.com";
                WebMail.Password = "YOUR_APP_PASSWORD";
                WebMail.From = "YOUR_EMAIL@gmail.com";

                string sisu = $@"
            Tere, {nimi}!<br/><br/>
            Sa registreerisid edukalt kursusele 
            <b>{kursus}</b>.<br/><br/>
            Ootame sind väga!<br/><br/>
            Parimate soovidega,<br/>
            Keeltekool
        ";

                WebMail.Send(
                    to: email,
                    subject: "Kinnituskiri kursusele: " + kursus,
                    body: sisu,
                    isBodyHtml: true
                );
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("E-maili viga: " + ex.Message);
            }
        }

    }
}
