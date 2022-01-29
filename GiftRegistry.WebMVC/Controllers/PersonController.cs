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
        // MAIN USER ONLY

        //// GET: Create
        //public ActionResult Create()
        //{
        //    PersonCreate model = new PersonCreate();
        //    model.Birthdate = DateTime.Now;

        //    return View(model);
        //}

        //// POST: Create
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create(PersonCreate model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        model.Birthdate = (model.Birthdate is null) ? DateTime.Now : model.Birthdate;
        //        return View(model);
        //    }

        //    var service = CreatePersonService();

        //    if (service.CreatePerson(model))
        //    {
        //        TempData["SaveResult"] = "Your person was created.";
        //        return RedirectToAction("Index");
        //    }

        //    ModelState.AddModelError("", "Person could not be created.");

        //    model.Birthdate = (model.Birthdate is null) ? DateTime.Now : model.Birthdate;
        //    return View(model);
        //}

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
                    ProfilePicture = detail.ProfilePicture,
                    OldProfilePicture = detail.ProfilePicture
                };

            return View(model);
        }

        // POST: Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(PersonEdit model)
        {
            if (!ModelState.IsValid) return View(model);

            //if (model.PersonID != id)
            //{
            //    ModelState.AddModelError("", "Id Mismatch");
            //    model.Birthdate = (model.Birthdate is null) ? DateTime.Now : model.Birthdate;
            //    return View(model);
            //}

            HttpPostedFileBase file = Request.Files["ImageData"];
            if(file.ContentLength != 0)
            {
                model.ProfilePicture = ConvertToBytes(file);
            }else
            {
                model.ProfilePicture = model.OldProfilePicture;
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

        // FRIENDS LIST ONLY

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

        //// GET: Delete
        //[ActionName("Delete")]
        //public ActionResult Delete(int id)
        //{
        //    var svc = CreatePersonService();
        //    var model = svc.GetPersonById(id);

        //    return View(model);
        //}

        //// POST: Delete
        //[HttpPost]
        //[ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeletePost(int id)
        //{
        //    var service = CreatePersonService();

        //    service.DeletePerson(id);

        //    TempData["SaveResult"] = "Your person was deleted.";

        //    return RedirectToAction("Index");
        //}

        private PersonService CreatePersonService()
        {
            var userId = Guid.Parse(User.Identity.GetUserId());
            var service = new PersonService(userId);
            return service;
        }

        public ActionResult GetProfilePicture(byte[] oldImage)
        {
            // Check if the user has uploaded any new images yet
            HttpPostedFileBase file = Request.Files["ImageData"];
            if (file != null && file.ContentLength != 0)
            {
                return File(ConvertToBytes(file), "image/png");
            }

            // If not, get the old image
            return File(oldImage, "image/png");
        }

    }
}