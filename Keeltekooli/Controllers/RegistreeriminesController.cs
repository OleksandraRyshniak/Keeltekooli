using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace Keeltekooli.Models
{
    public class RegistreeriminesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Registreerimines
        public ActionResult Index(int? koolitus)
        {

            var query = db.Registreerimine
            .Include(r => r.Koolitus.Keelekursus)
            .Include(r => r.User);
            // Если пользователь Учитель
            if (User.IsInRole("Opetaja"))
            {
                query = query.Where(r => r.Staatus == "Kinnitatud");
            }
            else{}

            if (koolitus != null)
            {
                query = query.Where(r => r.KoolitusId == koolitus);

                var koolitusObj = db.Koolitus
                    .Include(k => k.Keelekursus)
                    .FirstOrDefault(k => k.Id == koolitus);

                if (koolitusObj != null)
                {
                    ViewBag.KoolitusNimi = koolitusObj.Keelekursus.Nimetus;
                }
                else
                {
                    ViewBag.KoolitusNimi = "Tundmatu koolitus";
                }
            }
            else
            {
                ViewBag.KoolitusNimi = "Kõik koolitused";
            }

            var registreerimised = query
                .Select(r => new RegistreerimineViewModel
                {
                    Id = r.Id,
                    KoolitusId = r.KoolitusId,
                    Koolitus = r.Koolitus,
                    Nimi = r.User.UserName,
                    Email = r.User.Email,
                    Staatus = r.Staatus
                })
                .ToList();

            ViewBag.VaatamiselCount = registreerimised.Count;

            return View(registreerimised);
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

            // Заполняем email текущего пользователя (если авторизован)
            model.Email = User.Identity.GetUserName();

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
                // Внутри POST Create при возврате View(model) добавьте:
                var koolitus1 = db.Koolitus
                    .Include(k => k.Keelekursus)
                    .Include(k => k.Opetaja)
                    .FirstOrDefault(k => k.Id == model.KoolitusId);
                ViewBag.Koolitus = koolitus1;
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
                    UserName = model.Nimi,   // имя сохраняем сюда
                    Email = model.Email
                };

                db.Users.Add(user);
            }
            else
            {
                // если пользователь уже есть — обновляем имя
                user.UserName = model.Nimi;
            }

            db.SaveChanges();

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

            SaadaEmail(user, koolitus, true, "languagebridge.png"); // true — подтверждение участия

            return RedirectToAction("Tanan" , new { id = registreerimine.Id });
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,KoolitusId,ApplicationUserId,Staatus")] Registreerimine registreerimine)
        {
            if (ModelState.IsValid)
            {
                db.Entry(registreerimine).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", new { koolitus = registreerimine.KoolitusId });
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

        public ActionResult Tanan(int? id)
        {
            if (id == null)
                return RedirectToAction("Index");

            var reg = db.Registreerimine
                        .Include(r => r.Koolitus.Keelekursus)
                        .Include(r => r.Koolitus.Opetaja)
                        .FirstOrDefault(r => r.Id == id);

            if (reg == null)
                return RedirectToAction("Index");

            ViewBag.Keelekursus = reg.Koolitus.Keelekursus?.Nimetus ?? "–";
            ViewBag.Opetaja = reg.Koolitus.Opetaja?.Nimi ?? "–";
            ViewBag.Staatus = reg.Staatus;

            return View(reg);
        }
        //https://myaccount.google.com/apppasswords

        private void SaadaEmail(ApplicationUser user, Koolitus koolitus, bool onkutse, string fotoPath)
        {
            try
            {
                string failiTee = Path.Combine(Server.MapPath("~/Images/"), fotoPath);

                WebMail.SmtpServer = "smtp.gmail.com";
                WebMail.SmtpPort = 587;
                WebMail.EnableSsl = true;
                WebMail.UserName = "oleksandraryshniak@gmail.com";
                WebMail.Password = "ibzp beef fecd umby";
                WebMail.From = "oleksandraryshniak@gmail.com";

                string sisu = onkutse
                    ? $"Tere, {user.UserName}!<br/><br/>Sinu registreerumine kursusele <b>{koolitus.Keelekursus.Nimetus}</b> on salvestatud. Ootame sind väga!<br/><br/>Kohtumiseni!"
                    : $"Tere, {user.UserName}!<br/><br/>Sinu registreerumine kursusele <b>{koolitus.Keelekursus.Nimetus}</b> on salvestatud. Kahju, et sa ei tule!<br/><br/>Kõike head!";

                WebMail.Send(
                    to: user.Email,
                    subject: $"Kinnitus registreerumisest kursusele {koolitus.Keelekursus.Nimetus}",
                    body: sisu,
                    isBodyHtml: true,
                    filesToAttach: new string[] { failiTee }
                );
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("E-maili saatmise viga: " + ex.Message);
            }
        }

        private void SaadaEmail_ok(ApplicationUser user, Koolitus koolitus, bool onkutse, string fotoPath)
        {
            try
            {
                string failiTee = Path.Combine(Server.MapPath("~/Images/"), fotoPath);

                WebMail.SmtpServer = "smtp.gmail.com";
                WebMail.SmtpPort = 587;
                WebMail.EnableSsl = true;
                WebMail.UserName = "oleksandraryshniak@gmail.com";
                WebMail.Password = "ibzp beef fecd umby";
                WebMail.From = "oleksandraryshniak@gmail.com";

                string sisu = onkutse
                    ? $"Tere, {user.UserName}!<br/><br/>Sinu registreerumine kursusele <b>{koolitus.Keelekursus.Nimetus}</b> on salvestatud. Ootame sind väga!<br/><br/>Kohtumiseni!"
                    : $"Tere, {user.UserName}!<br/><br/>Sinu registreerumine kursusele <b>{koolitus.Keelekursus.Nimetus}</b> on salvestatud. Kahju, et sa ei tule!<br/><br/>Kõike head!";

                WebMail.Send(
                    to: user.Email,
                    subject: $"Kinnitus registreerumisest kursusele {koolitus.Keelekursus}",
                    body: sisu,
                    isBodyHtml: true,
                 filesToAttach: new string[] { failiTee }
                );
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("E-maili saatmise viga: " + ex.Message);
            }
        }

        public ActionResult VahetaStaatus(int id)
        {
            var registreerimine = db.Registreerimine.Find(id);
            if (registreerimine == null)
                return HttpNotFound();

            var koolitus = db.Koolitus.Find(registreerimine.KoolitusId);
            if (koolitus == null)
            {
                ModelState.AddModelError("", "Valitud grupp ei eksisteeri!");
                ViewBag.KoolitusId = new SelectList(db.Koolitus, "Id", "Nimi", registreerimine.KoolitusId);
                return View(registreerimine);
            }

            // Проверяем пользователя
            var user = db.Users.SingleOrDefault(u => u.Id == registreerimine.ApplicationUserId);

            if (user == null)
            {
                // Если пользователь не найден, можно обработать ошибку или создать нового пользователя
                ModelState.AddModelError("", "Kasutajat ei leitud!");
                return View(registreerimine);
            }

            // Логика смены статуса
            if (registreerimine.Staatus == "Vaatamisel")
                registreerimine.Staatus = "Kinnitatud";

            SaadaEmail_ok(user, koolitus, true, "languagebridge.png");
            db.SaveChanges();

            return RedirectToAction("Index", new { koolitus = registreerimine.KoolitusId });
        }
    }
}
