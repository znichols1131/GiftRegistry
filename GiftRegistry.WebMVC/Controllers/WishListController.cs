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
    [Authorize]
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
            WishListCreate model = new WishListCreate();
            return PartialView("_WishListCreatePartial", model);
        }

        // POST: Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Create(WishListCreate model)
        {
            bool successful;
            JsonResult jsonModel;

            if (!ModelState.IsValid)
            {
                successful = false;
                jsonModel = Json(new { successful }, JsonRequestBehavior.AllowGet);
                return jsonModel;
            }

            var service = CreateWishListService();

            if (service.CreateWishList(model))
            {
                TempData["SaveResult"] = "Your wish list was created.";
                successful = true;
                jsonModel = Json(new { successful }, JsonRequestBehavior.AllowGet);
                return jsonModel;
            }

            ModelState.AddModelError("", "Wish list could not be created.");

            successful = false;
            jsonModel = Json(new { successful }, JsonRequestBehavior.AllowGet);
            return jsonModel;
        }

        // GET: Detail
        public ActionResult Details(int id, string search, string sort)
        {            
            var service = CreateWishListService();

            var model = string.IsNullOrWhiteSpace(search) ? service.GetWishListByID(id) : service.GetWishListByIDAndSearch(id, search); 

            ViewBag.UserGUID = Guid.Parse(User.Identity.GetUserId());

            // Sorting options
            List<SelectListItem> sortOptions = new List<SelectListItem>();
            sortOptions.Add(new SelectListItem
            {
                Text = "Name (A to Z)",
                Value = "name_asc",
                Selected = true
            });
            sortOptions.Add(new SelectListItem
            {
                Text = "Name (Z to A)",
                Value = "name_desc"
            });
            ViewBag.SortOptions = sortOptions;

            if(!string.IsNullOrWhiteSpace(sort))
            {
                switch(sort)
                {
                    case "name_asc":
                        model.Gifts = model.Gifts.OrderBy(g => g.Name.ToLower()).ToList();
                        break;
                    case "name_desc":
                        model.Gifts = model.Gifts.OrderByDescending(g => g.Name).ToList(); 
                        break;
                    default:
                        model.Gifts = model.Gifts.OrderBy(g => g.Name.ToLower()).ToList(); 
                        break;
                }
            }

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


            return PartialView("_WishListEditPartial", model);
        }

        // POST: Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Edit(int id, WishListEdit model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { successful = false }, JsonRequestBehavior.AllowGet);
            }

            if (model.WishListID != id)
            {
                ModelState.AddModelError("", "Id Mismatch");
                return Json(new { successful = false }, JsonRequestBehavior.AllowGet);
            }

            var service = CreateWishListService();

            if (service.UpdateWishList(model))
            {
                TempData["SaveResult"] = "Your wish list was updated.";
                return Json(new { successful = true }, JsonRequestBehavior.AllowGet);
            }

            ModelState.AddModelError("", "Your wish list could not be updated.");
            return Json(new { successful = false }, JsonRequestBehavior.AllowGet);
        }

        // GET: Delete
        [HttpGet]
        [ActionName("Delete")]
        public ActionResult Delete(int id)
        {            
            var service = CreateWishListService();

            var model = service.GetWishListByID(id);
            return PartialView("_WishListDeletePartial", model);
        }

        // POST: Delete
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public JsonResult DeletePost(int id)
        {            
            var service = CreateWishListService();
            bool successful = service.DeleteWishList(id);

            if(successful)
                TempData["SaveResult"] = "Your wish list was deleted.";

            var jsonModel = Json(new { successful }, JsonRequestBehavior.AllowGet);

            return jsonModel;
        }

        private WishListService CreateWishListService()
        {
            if(!User.Identity.IsAuthenticated)
                return null;

            var userId = Guid.Parse(User.Identity.GetUserId());
            var service = new WishListService(userId);
            return service;
        }
    }
}