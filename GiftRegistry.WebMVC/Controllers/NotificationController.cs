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

            return View(model);
        }

        // GET: Delete
        [ActionName("Delete")]
        public ActionResult Delete(int id)
        {
            var service = CreateNotificationService();

            var model = service.GetNotificationByID(id);

            return View(model);
        }

        // POST: Delete
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeletePost(int id)
        {
            var service = CreateNotificationService();

            service.DeleteNotification(id);

            TempData["SaveResult"] = "Your notification was deleted.";

            return RedirectToAction("Index");
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