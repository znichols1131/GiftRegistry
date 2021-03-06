using GiftRegistry.Models;
using GiftRegistry.Services;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GiftRegistry.WebMVC.Controllers
{
    [Authorize]
    public class PersonController : Controller
    {
        // GET: Edit
        public ActionResult Edit()
        {            
            var service = CreatePersonService();
            var detail = service.GetCurrentPerson();

            if(detail is null)
            {
                // Create a new person
                service.CreatePerson(new PersonCreate() { FirstName = "First Name", LastName = "Last Name" });
                detail = service.GetCurrentPerson();
            }

            var model =
                new PersonEdit
                {
                    PersonID = detail.PersonID,
                    FirstName = detail.FirstName,
                    LastName = detail.LastName,
                    Birthdate = (detail.Birthdate is null) ? DateTime.Now : detail.Birthdate,
                    Image = detail.Image,
                    ImageID = detail.Image.ImageID
                };       

            return View(model);
        }

        // POST: Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(PersonEdit model)
        {            
            if (!ModelState.IsValid) return View(model);

            var imageService = CreateImageService();
            if (model.ImageID == -1)
            {
                model.Image = imageService.GetLatestImageForUser();

                if (model.Image is null)
                {
                    // Invalid, get a default image
                    model.Image = imageService.CreateAndReturnRandomImage(false);
                }
            }
            else
            {
                model.Image = imageService.GetImageByID(model.ImageID);
            }

            var service = CreatePersonService();
            if (service.UpdatePerson(model))
            {
                TempData["SaveResult"] = "Your person was updated.";
                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", "Your person could not be updated.");
            model.Birthdate = (model.Birthdate is null) ? DateTime.Now : model.Birthdate;
            return View(model);
        }

        // POST: Edit
        [HttpPost]
        [ActionName("EditNoRedirect")]
        [ValidateAntiForgeryToken]
        public JsonResult Edit_NoRedirect(PersonEdit model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { successful = false }, JsonRequestBehavior.AllowGet);
            }

            var imageService = CreateImageService();
            if (model.ImageID == -1)
            {
                model.Image = imageService.GetLatestImageForUser();

                if (model.Image is null)
                {
                    // Invalid, get a default image
                    model.Image = imageService.CreateAndReturnRandomImage(false);
                }
            }
            else
            {
                model.Image = imageService.GetImageByID(model.ImageID);
            }

            var service = CreatePersonService();
            if (service.UpdatePerson(model))
            {
                TempData["SaveResult"] = "Your person was updated.";
                return Json(new { successful = true }, JsonRequestBehavior.AllowGet);
            }

            ModelState.AddModelError("", "Your person could not be updated.");
            model.Birthdate = (model.Birthdate is null) ? DateTime.Now : model.Birthdate;
            return Json(new { successful = false }, JsonRequestBehavior.AllowGet);
        }

        // ADD FRIENDS LIST ONLY

        // GET: Person
        public ActionResult Index(string search)
        {            
            var service = CreatePersonService();

            var model = (string.IsNullOrWhiteSpace(search)) ? service.GetStrangers() : service.GetStrangersForSearchString(search);

            return View(model);
        }

        // GET: Detail
        public ActionResult Details(int id)
        {            
            var service = CreatePersonService();

            var model = service.GetPersonById(id);

            return View(model);
        }

        // GET: Detail
        [HttpGet]
        [ActionName("FriendDetails")]
        public ActionResult FriendDetails(int personID)
        {
            var service = CreateFriendService();

            var friendID = service.GetFriendIDByPersonID(personID);

            if(friendID != -1)
                return RedirectToAction("Details", "Friend", new { id = friendID });

            // Worst case
            return RedirectToAction("Index", "Friend");
        }

        private byte[] ConvertToBytes(HttpPostedFileBase image)
        {
            using (var reader = new BinaryReader(image.InputStream))
            {
                return reader.ReadBytes(image.ContentLength);
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

        private FriendService CreateFriendService()
        {
            if (!User.Identity.IsAuthenticated)
                return null;

            var userId = Guid.Parse(User.Identity.GetUserId());
            var service = new FriendService(userId);
            return service;
        }

        private ImageService CreateImageService()
        {
            var userId = Guid.Parse(User.Identity.GetUserId());
            var service = new ImageService(userId);
            return service;
        }
    }
}