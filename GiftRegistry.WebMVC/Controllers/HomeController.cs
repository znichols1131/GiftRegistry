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
        public ActionResult Index(string time)
        {
            var service = CreateHomeService();
            IEnumerable<EventListItem> model;

            if(service is null)
            {
                model = null;
            }else
            {
                // Default setting
                time = string.IsNullOrWhiteSpace(time) ? "month03" : time;
                model = service.GetEventsWithinTime(time);
                //model = string.IsNullOrWhiteSpace(time) ? service.GetEvents() : service.GetEventsWithinTime(time);
            }

            // Sorting options
            List<SelectListItem> sortOptions = new List<SelectListItem>();
            sortOptions.Add(new SelectListItem
            {
                Text = "1 month",
                Value = "month01"
            });
            sortOptions.Add(new SelectListItem
            {
                Text = "3 months",
                Value = "month03",
                Selected = true
            });
            sortOptions.Add(new SelectListItem
            {
                Text = "6 months",
                Value = "month06"
            });
            sortOptions.Add(new SelectListItem
            {
                Text = "12 months",
                Value = "month12"
            });
            ViewBag.SortOptions = sortOptions;

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
            if (!User.Identity.IsAuthenticated)
                return null;

            try
            {
                var userId = Guid.Parse(User.Identity.GetUserId());
                var service = new HomeService(userId);
                return service;
            }catch
            {
                return null;
            }
        }
    }
}