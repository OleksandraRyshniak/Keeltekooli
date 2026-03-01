using Keeltekooli.Models;
using System.Data.Entity;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace Keeltekooli.Controllers
{
    public class SendEmailController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // =========================
        // GET: SendEmail
        // =========================
        public ActionResult SendEmail(int? id)
        {
            if (id == null)
                return HttpNotFound();

            var model = db.Registreerimine
                .Include(r => r.User)
                .Where(r => r.Id == id)
                .Select(r => new EmailViewModel
                {
                    Id = r.Id,
                    To = r.User.Email
                })
                .FirstOrDefault();

            if (model == null)
                return HttpNotFound();

            return View(model);
        }

        // =========================
        // POST: SendEmail
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SendEmail(EmailViewModel model)
        {
            if (string.IsNullOrEmpty(model.To))
            {
                ModelState.AddModelError("", "Saaja e-posti aadress on tühi!");
                return View(model);
            }

            try
            {
                WebMail.SmtpServer = "smtp.gmail.com";
                WebMail.SmtpPort = 587;
                WebMail.EnableSsl = true;
                WebMail.UserName = "oleksandraryshniak@gmail.com";
                WebMail.Password = "qppv odbp qvjs gvuc"; // ← настоящий App Password
                WebMail.From = "oleksandraryshniak@gmail.com";

                WebMail.Send(
                    to: model.To,
                    subject: model.Subject,
                    body: model.Body,
                    isBodyHtml: true
                );

                ViewBag.Message = "E-kiri on edukalt saadetud!";
                ModelState.Clear();

                return View(new EmailViewModel { To = model.To });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Saateviga: " + ex.Message);
                return View(model);
            }
        }

        public ActionResult SendEmailAll()
        {
            return View();
        }

        // ======================
        // POST (отправка всем)
        // ======================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SendEmailToAll(EmailViewModel model)
        {
            try
            {
                var emails = db.Users
                               .Where(u => u.Email != null)
                               .Select(u => u.Email)
                               .ToList();

                if (!emails.Any())
                {
                    ModelState.AddModelError("", "Pole kasutajaid saatmiseks!");
                    return View(model);
                }

                WebMail.SmtpServer = "smtp.gmail.com";
                WebMail.SmtpPort = 587;
                WebMail.EnableSsl = true;
                WebMail.UserName = "oleksandraryshniak@gmail.com";
                WebMail.Password = "qppv odbp qvjs gvuc";
                WebMail.From = "oleksandraryshniak@gmail.com";

                // Отправка одним письмом через BCC
                WebMail.Send(
                    to: "oleksandraryshniak@gmail.com",
                    bcc: string.Join(",", emails),
                    subject: model.Subject,
                    body: model.Body,
                    isBodyHtml: true
                );

                ViewBag.Message = "E-mail saadetud kõigile kasutajatele!";
                ModelState.Clear();

                return View();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Viga: " + ex.Message);
                return View(model);
            }
        }
    }
}

