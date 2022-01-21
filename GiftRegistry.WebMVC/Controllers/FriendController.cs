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
    public class FriendController : Controller
    {
        // GET: Friend
        public ActionResult Index()
        {
            var service = CreateFriendService();
            var model = service.GetFriends();
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

            if (service.CreateFriend(model))
            {
                TempData["SaveResult"] = "Your friend was created.";
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", "Friend could not be created.");

            return View(model);
        }

        // GET: Detail
        public ActionResult Details(int id)
        {
            var svc = CreateFriendService();
            var model = svc.GetFriendByID(id);

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
                };

            return View(model);
        }

        // POST: Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, FriendEdit model)
        {
            if (!ModelState.IsValid) return View(model);

            if (model.PersonID != id)
            {
                ModelState.AddModelError("", "Id Mismatch");
                return View(model);
            }

            var service = CreateFriendService();

            if (service.UpdateFriend(model))
            {
                TempData["SaveResult"] = "Your friend was updated.";
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", "Your friend could not be updated.");
            return View(model);
        }  

        // GET: Delete
        [ActionName("Delete")]
        public ActionResult Delete(int id)
        {
            var svc = CreateFriendService();
            var model = svc.GetFriendByID(id);

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
            var userId = Guid.Parse(User.Identity.GetUserId());
            var service = new FriendService(userId);
            return service;
        }
    }
}