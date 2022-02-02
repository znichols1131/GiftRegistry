using GiftRegistry.Models;
using GiftRegistry.Services;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GiftRegistry.WebMVC.Controllers
{
    [Authorize]
    public class GiftController : Controller
    {
        // GET: Create
        public ActionResult Create(int id)
        {            
            GiftCreate model = new GiftCreate();

            model.WishListID = id;

            var controller = CreateImageController();
            if (controller != null)
            {
                model.Image = controller.CreateRandomImage();
                return View(model);
            }

            model.Image = new ImageModel();
            return View(model);
        }

        // POST: Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(GiftCreate model)
        {            
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Get image
            var controller = CreateImageController();

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
                    model.Image = controller.CreateRandomImage();
                }
            }
            else
            {
                model.Image = controller.GetImageForID(model.ImageID, Guid.Parse(User.Identity.GetUserId()));
            }

            var service = CreateGiftService();

            if (service.CreateGift(model))
            {
                TempData["SaveResult"] = "Your gift was created.";

                return RedirectToAction("Details", "WishList", new { id = model.WishListID });
            }

            ModelState.AddModelError("", "Gift could not be created.");

            return View(model);
        }

        // GET: Detail
        public ActionResult Details(int id)
        {
            var service = CreateGiftService();

            var model = service.GetGiftByID(id);

            ViewBag.UserGUID = Guid.Parse(User.Identity.GetUserId());

            return View(model);
        }

        // GET: Edit
        public ActionResult Edit(int id)
        {
            var service = CreateGiftService();

            var detail = service.GetGiftByID(id);
            var model =
                new GiftEdit
                {
                    GiftID = detail.GiftID,
                    Name = detail.Name,
                    Description = detail.Description,
                    SourceURL = detail.SourceURL,
                    QtyDesired = detail.QtyDesired,
                    WishListID = detail.WishListID,
                    WishList = detail.WishList,
                    Image = detail.Image
                };

            return View(model);
        }

        // POST: Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, GiftEdit model)
        {            
            if (!ModelState.IsValid) return View(model);

            if (model.GiftID != id)
            {
                ModelState.AddModelError("", "Id Mismatch");
                return View(model);
            }

            // Get image
            var controller = CreateImageController();
            if(controller is null)
                return RedirectToAction("Index", "Home");

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
                    model.Image = controller.CreateRandomImage();
                }
            }
            else
            {
                model.Image = controller.GetImageForID(model.ImageID, Guid.Parse(User.Identity.GetUserId()));
            }

            var service = CreateGiftService();
            if (service is null)
                return RedirectToAction("Index", "Home");

            if (service.UpdateGift(model))
            {
                TempData["SaveResult"] = "Your gift was updated.";
                return RedirectToAction("Details", "WishList", new { id = model.WishListID });
            }

            ModelState.AddModelError("", "Your gift could not be updated.");
            return View(model);
        }

        // GET: Delete
        [ActionName("Delete")]
        public ActionResult Delete(int id)
        {            
            var service = CreateGiftService();

            var model = service.GetGiftByID(id);

            return View(model);
        }

        // POST: Delete
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeletePost(int id)
        {            
            var service = CreateGiftService();

            var model = service.GetGiftByID(id);

            service.DeleteGift(id);

            TempData["SaveResult"] = "Your gift was deleted.";

            return RedirectToAction("Details", "WishList", new { id = model.WishListID });
        }

        private byte[] ConvertToBytes(HttpPostedFileBase image)
        {
            using (var reader = new BinaryReader(image.InputStream))
            {
                return reader.ReadBytes(image.ContentLength);
            }
        }

        private GiftService CreateGiftService()
        {
            if (!User.Identity.IsAuthenticated)
                return null;

            var userId = Guid.Parse(User.Identity.GetUserId());
            var service = new GiftService(userId);
            return service;
        }



        private PersonService CreatePersonService()
        {
            if (!User.Identity.IsAuthenticated)
                return null;

            var userId = Guid.Parse(User.Identity.GetUserId());
            var service = new PersonService(userId);
            return service;
        }

        private _ImageUploadController CreateImageController()
        {
            if (!User.Identity.IsAuthenticated)
                return null;

            var controller = new _ImageUploadController();
            controller._isProfilePicture = false;
            return controller;
        }
    }
}