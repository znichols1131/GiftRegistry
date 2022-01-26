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
    public class TransactionController : Controller
    {
        // GET: Transaction
        public ActionResult Index()
        {
            var service = CreateTransactionService();
            var model = service.GetAllTransactionsForUser();
            return View(model);
        }

        // GET: Create
        public ActionResult Create(int giftID)
        {
            TransactionCreate model = new TransactionCreate();
            var transactionService = CreateTransactionService();

            var giftService = CreateGiftService();
            GiftDetail detail = giftService.GetGiftByID(giftID);
            model.WishListID = detail.WishListID;
            model.GiftName = detail.Name;
            model.RecipientName = detail.WishList.Owner.FullName;

            model.GiftID = giftID;
            model.GiverID = transactionService.GetCurrentUserID();

            return View(model);
        }

        // POST: Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TransactionCreate model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var service = CreateTransactionService();

            if (service.CreateTransaction(model))
            {
                TempData["SaveResult"] = "Your transaction was created.";

                return RedirectToAction("Details", "WishList", new { id = model.WishListID });
            }

            ModelState.AddModelError("", "Transaction could not be created.");

            return View(model);
        }

        // GET: Detail
        public ActionResult Details(int id)
        {
            var svc = CreateTransactionService();
            var model = svc.GetTransactionByID(id);

            ViewBag.UserGUID = Guid.Parse(User.Identity.GetUserId());

            return View(model);
        }

        // GET: Edit
        public ActionResult Edit(int id)
        {
            var service = CreateTransactionService();
            var detail = service.GetTransactionByID(id);
            var model =
                new TransactionEdit
                {
                    TransactionID = detail.TransactionID,
                    DateCreated = detail.DateCreated,
                    DateModified = detail.DateModified,
                    QtyGiven = detail.QtyGiven,
                    GiftID = detail.GiftID,
                    GiftName = detail.Gift.Name,
                    RecipientName = detail.RecipientName,
                    Gift = detail.Gift,
                    GiverID = detail.GiverID,
                    Giver = detail.Giver
                };

            return View(model);
        }

        // POST: Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, TransactionEdit model)
        {
            if (!ModelState.IsValid) return View(model);

            if (model.TransactionID != id)
            {
                ModelState.AddModelError("", "Id Mismatch");
                return View(model);
            }

            var service = CreateTransactionService();

            if (service.UpdateTransaction(model))
            {
                TempData["SaveResult"] = "Your transaction was updated.";
                return RedirectToAction("Index"); 
            }

            ModelState.AddModelError("", "Your transaction could not be updated.");
            return View(model);
        }

        // GET: Delete
        [ActionName("Delete")]
        public ActionResult Delete(int id)
        {
            var svc = CreateTransactionService();
            var model = svc.GetTransactionByID(id);

            return View(model);
        }

        // POST: Delete
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeletePost(int id)
        {
            var service = CreateTransactionService();
            var model = service.GetTransactionByID(id);

            service.DeleteTransaction(id);

            TempData["SaveResult"] = "Your transaction was deleted.";

            return RedirectToAction("Index");
        }

        private TransactionService CreateTransactionService()
        {
            var userId = Guid.Parse(User.Identity.GetUserId());
            var service = new TransactionService(userId);
            return service;
        }

        private GiftService CreateGiftService()
        {
            var userId = Guid.Parse(User.Identity.GetUserId());
            var service = new GiftService(userId);
            return service;
        }
    }
}