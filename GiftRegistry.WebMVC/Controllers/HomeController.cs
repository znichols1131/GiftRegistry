using GiftRegistry.Models;
using GiftRegistry.Services;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GiftRegistry.WebMVC.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var service = CreateHomeService();
            var model = service.GetEvents();

            ViewBag.DateString = GetDateString();
            return View(model);
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

        private string GetDateString()
        {
            DateTime today = DateTime.Now;
            return today.ToString("MMMM dd, yyyy");
        }

        private HomeService CreateHomeService()
        {
            var userId = Guid.Parse(User.Identity.GetUserId());
            var service = new HomeService(userId);
            return service;
        }
    }
}