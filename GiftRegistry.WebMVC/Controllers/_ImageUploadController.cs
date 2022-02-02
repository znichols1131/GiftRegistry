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
    public class _ImageUploadController : Controller
    {
        public bool _isProfilePicture { get; set; }
        private Guid _userGUID;

        public _ImageUploadController()
        {
            _isProfilePicture = true;

            if(User != null)
            {
                try { _userGUID = Guid.Parse(User.Identity.GetUserId()); } catch { }
            }
        }
        public _ImageUploadController(Guid userGUID)
        {
            _isProfilePicture = true;
            _userGUID = userGUID;
        }

        [HttpGet]
        public JsonResult CreateRandomImageJSON()
        {
            ImageModel model = CreateRandomImage();
            if (model is null)
                return Json(null, JsonRequestBehavior.AllowGet);

            var imageSrc = String.Format("data:image/gif;base64,{0}", Convert.ToBase64String(model.ImageData));
            var newModel = new { model.ImageID, imageSrc };

            var jsonModel = Json(newModel, JsonRequestBehavior.AllowGet);

            return jsonModel;
        }

        public ImageModel CreateRandomImage()
        {
            // Get service and clear all images for this user (meant to be temporary)
            var service = CreateImageService();
            service.DeleteImagesForUser();

            if (!service.CreateDefaultImage(_isProfilePicture))
            {
                return null;
            }
            return service.GetLatestImageForUser();
        }

        [HttpPost]
        public JsonResult CreateUploadedImageJSON(HttpPostedFileBase file)
        {
            // Get service and clear all images for this user (meant to be temporary)
            var service = CreateImageService();
            service.DeleteImagesForUser();

            ImageModel model = new ImageModel();

            // Try to get uploaded file, otherwise get a random image
            if (file != null && file.ContentLength != 0)
            {
                model.ImageData = ConvertToBytes(file);
            }
            else
            {
                if (!service.CreateDefaultImage(_isProfilePicture))
                {
                    return Json(null, JsonRequestBehavior.AllowGet);
                }
                model = service.GetLatestImageForUser();
            }

            var imageSrc = String.Format("data:image/gif;base64,{0}", Convert.ToBase64String(model.ImageData));
            var newModel = new { model.ImageID, imageSrc };

            var jsonModel = Json(newModel, JsonRequestBehavior.AllowGet);

            return jsonModel;
        }

        public ImageModel GetLatestImageForUser()
        {
            var service = CreateImageService();
            return service.GetLatestImageForUser();
        }

        public ImageModel GetLatestImageForUser(Guid userID)
        {
            var service = CreateImageService(userID);
            return service.GetLatestImageForUser();
        }

        public ImageModel GetImageForID(int id)
        {
            var service = CreateImageService();
            return service.GetImageByID(id);
        }
        public ImageModel GetImageForID(int id, Guid userID)
        {
            var service = CreateImageService(userID);
            return service.GetImageByID(id);
        }

        private ImageService CreateImageService()
        {
            Guid userId;
            try
            {
                if (User != null)
                {
                    userId = Guid.Parse(User.Identity.GetUserId());
                }
                else
                {
                    userId = _userGUID;
                }
            }
            catch
            {
                userId = _userGUID;
            }            

            var service = new ImageService(userId);
            return service;
        }

        private ImageService CreateImageService(Guid userID)
        {
            var service = new ImageService(userID);
            return service;
        }

        private byte[] ConvertToBytes(HttpPostedFileBase image)
        {
            using (var reader = new BinaryReader(image.InputStream))
            {
                return reader.ReadBytes(image.ContentLength);
            }
        }
    }
}