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
                // Image was uploaded and doesn't exist in database yet
                HttpPostedFileBase file = Request.Files["imageFile"];
                if (file != null && file.ContentLength != 0)
                {
                    // Valid image file uploaded
                    ImageModel newImage = new ImageModel();
                    newImage.ImageData = ConvertToBytes(file);
                    model.Image = newImage;
                }
                else
                {
                    // Invalid, get a default image
                    model.Image = imageService.CreateAndReturnRandomImage(true);
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