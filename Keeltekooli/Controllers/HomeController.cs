using Keeltekooli.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace Keeltekooli.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Tanan(int? id)
        {
            if (id == null)
                return RedirectToAction("Index");

            var reg = db.Registreerimine
                        .Include(r => r.Koolitus)
                        .FirstOrDefault(r => r.Id == id);

            if (reg == null)
                return RedirectToAction("Index");

            ViewBag.Kursus = reg.Koolitus.Keelekursus.Nimetus;

            return View();
        }
    } 
}