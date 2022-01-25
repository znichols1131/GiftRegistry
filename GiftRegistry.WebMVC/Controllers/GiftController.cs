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
    public class GiftController : Controller
    {
        // GET: Gift
        //public ActionResult Index(int wishListID)
        //{
        //    var service = CreateGiftService();
        //    var model = service.GetGiftsForWishListID(wishListID);
        //    return View(model);
        //}

        // GET: Create
        public ActionResult Create(int id)
        {
            GiftCreate model = new GiftCreate();
            model.WishListID = id;
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
            var svc = CreateGiftService();
            var model = svc.GetGiftByID(id);

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
                    WishList = detail.WishList
                };

            return View(model);
        }

        // POST: Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, GiftEdit model)
        {
            if (!ModelState.IsValid) return View(model);

            if (model.WishListID != id)
            {
                ModelState.AddModelError("", "Id Mismatch");
                return View(model);
            }

            var service = CreateGiftService();

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
            var svc = CreateGiftService();
            var model = svc.GetGiftByID(id);

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

        private GiftService CreateGiftService()
        {
            var userId = Guid.Parse(User.Identity.GetUserId());
            var service = new GiftService(userId);
            return service;
        }
    }
}