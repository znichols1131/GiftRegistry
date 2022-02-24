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
            // Check if they have a Person object yet
            if (User.Identity.IsAuthenticated && !PersonExists())
            {
                return RedirectToAction("GetStarted", "Home");
            }

            var service = CreateHomeService();
            IEnumerable<EventListItem> model;

            if (service is null)
            {
                model = null;
            }
            else
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

        [Authorize]
        [HttpGet]
        [ActionName("GetStarted")]
        public ActionResult GetStarted()
        {
            // Get the person object
            var service = CreatePersonService();
            var person = service.GetCurrentPerson();

            // If person doesn't exist, make a new person
            if (person is null)
            {
                service.CreatePerson(new PersonCreate() { FirstName = "First Name", LastName = "Last Name" });
                person = service.GetCurrentPerson();
            }

            var model =
                new PersonEdit
                {
                    PersonID = person.PersonID,
                    FirstName = person.FirstName == "First Name" ? "" : person.FirstName,
                    LastName = person.LastName == "Last Name" ? "" : person.LastName,
                    Birthdate = person.Birthdate is null ? DateTime.Now : person.Birthdate,
                    Image = person.Image,
                    ImageID = person.Image.ImageID
                };

            return View(model);
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
            }
            catch
            {
                return null;
            }
        }
        private PersonService CreatePersonService()
        {
            if (!User.Identity.IsAuthenticated)
                return null;

            var userId = Guid.Parse(User.Identity.GetUserId());
            var service = new PersonService(userId);
            return service;
        }

        private bool PersonExists()
        {
            try
            {
                var service = CreatePersonService();
                var person = service.GetCurrentPerson();

                return (person != null && 
                    !string.IsNullOrWhiteSpace(person.FirstName) && person.FirstName.ToLower() != "first name" &&
                    !string.IsNullOrWhiteSpace(person.LastName) && person.LastName.ToLower() != "last name");
            }
            catch
            {
                return false;
            }
        }
    }
}