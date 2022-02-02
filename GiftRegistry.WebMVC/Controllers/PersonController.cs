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
                    Image = detail.Image
                };       

            return View(model);
        }

        // POST: Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(PersonEdit model)
        {
            if (!ModelState.IsValid) return View(model);

            var controller = CreateImageController();

            if(model.ImageID == -1)
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
                    model.Image = controller.CreateRandomImage();
                }
            }
            else
            {
                model.Image = controller.GetImageForID(model.ImageID, Guid.Parse(User.Identity.GetUserId()));
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

        private byte[] ConvertToBytes(HttpPostedFileBase image)
        {
            using (var reader = new BinaryReader(image.InputStream))
            {
               return reader.ReadBytes(image.ContentLength);
            }
        }

        // ADD FRIENDS LIST ONLY

        // GET: Person
        public ActionResult Index()
        {
            var service = CreatePersonService();
            var model = service.GetStrangers();
            return View(model);
        }

        // GET: Detail
        public ActionResult Details(int id)
        {
            var svc = CreatePersonService();
            var model = svc.GetPersonById(id);

            return View(model);
        }



        private PersonService CreatePersonService()
        {
            var userId = Guid.Parse(User.Identity.GetUserId());
            var service = new PersonService(userId);
            return service;
        }

        private _ImageUploadController CreateImageController()
        {
            var controller = new _ImageUploadController();
            controller._isProfilePicture = true;
            return controller;
        }
    }
}