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

            var imageService = CreateImageService();
            if (imageService != null)
            {
                model.Image = imageService.CreateAndReturnRandomImage(false);
                model.ImageID = model.Image.ImageID;
                return PartialView("_GiftCreatePartial", model);
            }

            model.Image = new ImageModel();
            return PartialView("_GiftCreatePartial", model);
        }

        // POST: Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Create(GiftCreate model)
        {            
            if (!ModelState.IsValid)
            {
                return Json(new { successful = false }, JsonRequestBehavior.AllowGet);
            }

            // Get image
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
                    model.Image = imageService.CreateAndReturnRandomImage(false);
                }
            }
            else
            {
                model.Image = imageService.GetImageByID(model.ImageID);
            }

            var service = CreateGiftService();

            if (service.CreateGift(model))
            {
                TempData["SaveResult"] = "Your gift was created.";

                return Json(new { successful = true }, JsonRequestBehavior.AllowGet);
            }

            ModelState.AddModelError("", "Gift could not be created.");

            return Json(new { successful = false }, JsonRequestBehavior.AllowGet);
        }

        // GET: Detail
        public ActionResult Details(int id)
        {
            var service = CreateGiftService();

            var model = service.GetGiftByID(id);

            ViewBag.UserGUID = Guid.Parse(User.Identity.GetUserId());

            return PartialView("_GiftDetailsPartial", model);
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
                    Image = detail.Image,
                    ImageID = detail.Image.ImageID
                };

            return PartialView("_GiftEditPartial", model);
        }

        // POST: Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Edit(int id, GiftEdit model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { successful = false }, JsonRequestBehavior.AllowGet);
            }

            if (model.GiftID != id)
            {
                ModelState.AddModelError("", "Id Mismatch");
                return Json(new { successful = false }, JsonRequestBehavior.AllowGet);
            }

            // Get image
            var imageService = CreateImageService();
            if(imageService is null)
                return Json(new { successful = false }, JsonRequestBehavior.AllowGet);

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
                    model.Image = imageService.CreateAndReturnRandomImage(false);
                }
            }
            else
            {
                model.Image = imageService.GetImageByID(model.ImageID);
            }

            var service = CreateGiftService();
            if (service is null)
                return Json(new { successful = false }, JsonRequestBehavior.AllowGet);

            if (service.UpdateGift(model))
            {
                TempData["SaveResult"] = "Your gift was updated.";
                return Json(new { successful = true }, JsonRequestBehavior.AllowGet);
            }

            ModelState.AddModelError("", "Your gift could not be updated.");
            return Json(new { successful = false }, JsonRequestBehavior.AllowGet);
        }

        // GET: Delete
        [ActionName("Delete")]
        public ActionResult Delete(int id)
        {            
            var service = CreateGiftService();

            var model = service.GetGiftByID(id);

            return PartialView("_GiftDeletePartial", model);
        }

        // POST: Delete
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeletePost(int id)
        {            
            var service = CreateGiftService();

            var model = service.GetGiftByID(id);

            bool successful = service.DeleteGift(id);

            TempData["SaveResult"] = "Your gift was deleted.";

            return Json(new { successful }, JsonRequestBehavior.AllowGet);
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

        private ImageService CreateImageService()
        {
            var userId = Guid.Parse(User.Identity.GetUserId());
            var service = new ImageService(userId);
            return service;
        }
    }
}