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
    public class WishListController : Controller
    {
        // GET: WishList
        public ActionResult Index()
        {
            var service = CreateWishListService();
            var model = service.GetWishListsForCurrentUser();
            return View(model);
        }

        // GET: Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(WishListCreate model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var service = CreateWishListService();

            if (service.CreateWishList(model))
            {
                TempData["SaveResult"] = "Your wish list was created.";
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", "Wish list could not be created.");

            return View(model);
        }

        // GET: Detail
        public ActionResult Details(int id)
        {
            var svc = CreateWishListService();
            var model = svc.GetWishListByID(id);

            return View(model);
        }

        // GET: Edit
        public ActionResult Edit(int id)
        {
            var service = CreateWishListService();
            var detail = service.GetWishListByID(id);
            var model =
                new WishListEdit
                {
                    WishListID = detail.WishListID,
                    OwnerID = detail.OwnerID,
                    OwnerGUID = detail.OwnerGUID,
                    Name = detail.Name,
                    Description = detail.Description,
                    DueDate = detail.DueDate,
                };

            return View(model);
        }

        // POST: Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, WishListEdit model)
        {
            if (!ModelState.IsValid) return View(model);

            if (model.WishListID != id)
            {
                ModelState.AddModelError("", "Id Mismatch");
                return View(model);
            }

            var service = CreateWishListService();

            if (service.UpdateWishList(model))
            {
                TempData["SaveResult"] = "Your wish list was updated.";
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", "Your wish list could not be updated.");
            return View(model);
        }

        // GET: Delete
        [ActionName("Delete")]
        public ActionResult Delete(int id)
        {
            var svc = CreateWishListService();
            var model = svc.GetWishListByID(id);

            return View(model);
        }

        // POST: Delete
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeletePost(int id)
        {
            var service = CreateWishListService();

            service.DeleteWishList(id);

            TempData["SaveResult"] = "Your wish list was deleted.";

            return RedirectToAction("Index");
        }

        private WishListService CreateWishListService()
        {
            var userId = Guid.Parse(User.Identity.GetUserId());
            var service = new WishListService(userId);
            return service;
        }
    }
}