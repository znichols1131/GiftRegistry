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
    public class NotificationController : Controller
    {
        // GET: Notification
        public ActionResult Index()
        {
            var service = CreateNotificationService();

            var model = service.GetNotificationsForUser();
            return View(model);
        }

        // GET: Detail
        public ActionResult Details(int id)
        {
            var service = CreateNotificationService();

            var model = service.GetNotificationByID(id);

            return PartialView("_NotificationDetailsPartial", model);
        }

        // GET: Delete
        [ActionName("Delete")]
        public ActionResult Delete(int id)
        {
            var service = CreateNotificationService();

            var model = service.GetNotificationByID(id);

            return PartialView("_NotificationDeletePartial", model);
        }

        // POST: Delete
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public JsonResult DeletePost(int id)
        {
            var service = CreateNotificationService();

            bool successful = service.DeleteNotification(id);

            TempData["SaveResult"] = "Your notification was deleted.";

            return Json(new { successful }, JsonRequestBehavior.AllowGet);
        }


        // POST: Accept Friend Request
        [HttpPost]
        [ActionName("AcceptFriendRequest")]
        public JsonResult AcceptFriendRequest(int id)
        {
            // Get original friend request
            // Fulfill original friend request
            // Add sender to recipient's friends
            var service = CreateNotificationService();
            var sender = service.GetNotificationByID(id);
            var friendID = service.AcceptFriendRequest(id);

            if(friendID < 0)
            {
                TempData["SaveResult"] = "Friend request could not be accepted.";
                return Json(new { successful = false }, JsonRequestBehavior.AllowGet);
            }

            TempData["SaveResult"] = "You accepted the friend request.";

            // Send recipient to the Update page for new friend
            return Json(new { successful = true , friendID = friendID }, JsonRequestBehavior.AllowGet);
        }

        // POST: Deny Friend Request
        [HttpPost]
        [ActionName("DenyFriendRequest")]
        public JsonResult DenyFriendRequest(int id)
        {
            // Get original friend request
            // Deny original friend request
            var service = CreateNotificationService();
            service.DenyFriendRequest(id);
            TempData["SaveResult"] = "You denied the friend request.";

            // Send recipient back to notifications
            return Json(new { successful = true }, JsonRequestBehavior.AllowGet);
        }


        private NotificationService CreateNotificationService()
        {
            if (!User.Identity.IsAuthenticated)
                return null;

            var userId = Guid.Parse(User.Identity.GetUserId());
            var service = new NotificationService(userId);
            return service;
        }
    }
}