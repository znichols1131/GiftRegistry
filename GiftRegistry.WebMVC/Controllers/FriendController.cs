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
    public class FriendController : Controller
    {
        // GET: Friend
        public ActionResult Index(string search)
        {            
            var service = CreateFriendService();

            var model = (string.IsNullOrWhiteSpace(search)) ? service.GetFriends() : service.GetFriendsForSearchString(search);

            return View(model);
        }

        // GET: Create
        public ActionResult Create(int id, string name)
        {
            FriendCreate model = new FriendCreate();

            model.PersonID = id;
            model.PersonName = name;

            return View(model);
        }

        // POST: Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(FriendCreate model)
        {            
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var service = CreateFriendService();

            // We won't actually create a friend here, we'll send a friend request
            if (service.SendFriendRequest(model))
            {
                TempData["SaveResult"] = "Your friend request was sent.";
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", "Friend request could not be created.");

            return View(model);
        }

        // GET: Detail
        public ActionResult Details(int id)
        {            
            var service = CreateFriendService();

            var model = service.GetFriendByID(id);

            return View(model);
        }

        // GET: Edit
        public ActionResult Edit(int id)
        {            
            var service = CreateFriendService();

            var detail = service.GetFriendByID(id);
            var model =
                new FriendEdit
                {
                    FriendID = detail.FriendID,
                    OwnerGUID = detail.OwnerGUID,
                    Relationship = detail.Relationship,
                    PersonID = detail.PersonID,
                    PersonName = detail.PersonName
                };

            return PartialView("_FriendEditPartial", model);
        }

        // POST: Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, FriendEdit model)
        {            
            if (!ModelState.IsValid)
            {
                return Json(new { successful = false }, JsonRequestBehavior.AllowGet);
            }

            if (model.FriendID != id)
            {
                ModelState.AddModelError("", "Id Mismatch");
                return Json(new { successful = false }, JsonRequestBehavior.AllowGet);
            }

            var service = CreateFriendService();
            if (service.UpdateFriend(model))
            {
                TempData["SaveResult"] = "Your friend was updated.";
                return Json(new { successful = true }, JsonRequestBehavior.AllowGet);
            }

            ModelState.AddModelError("", "Your friend could not be updated.");
            return Json(new { successful = false }, JsonRequestBehavior.AllowGet);
        }

        // GET: Delete
        [ActionName("Delete")]
        public ActionResult Delete(int id)
        {            
            var service = CreateFriendService();

            var model = service.GetFriendByID(id);

            return View(model);
        }

        // POST: Delete
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeletePost(int id)
        {            
            var service = CreateFriendService();

            service.DeleteFriend(id);

            TempData["SaveResult"] = "Your friend was deleted.";

            return RedirectToAction("Index");
        }

        private FriendService CreateFriendService()
        {
            if (!User.Identity.IsAuthenticated)
                return null;

            var userId = Guid.Parse(User.Identity.GetUserId());
            var service = new FriendService(userId);
            return service;
        }
    }
}