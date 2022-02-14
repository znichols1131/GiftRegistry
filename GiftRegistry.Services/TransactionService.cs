using GiftRegistry.Data;
using GiftRegistry.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GiftRegistry.Services
{
    public class TransactionService
    {
        private readonly Guid _userId;

        public TransactionService(Guid userId)
        {
            _userId = userId;
        }

        public bool CreateTransaction(TransactionCreate model)
        {
            var entity =
                new Transaction()
                {
                    DateCreated = DateTime.Now,
                    QtyGiven = model.QtyGiven,
                    GiftID = model.GiftID,
                    GiverID = model.GiverID
                };

            using (var ctx = new ApplicationDbContext())
            {
                ctx.Transactions.Add(entity);
                return ctx.SaveChanges() == 1;
            }
        }

        public IEnumerable<TransactionListItem> GetTransactionsForUserForGiftID(int giftID)
        {
            using (var ctx = new ApplicationDbContext())
            {
                var query =
                    ctx
                        .Transactions
                        .Include("Giver")
                        .Include("Gift")
                        .Include("Gift.WishList")
                        .Include("Gift.WishList.Owner")
                        .Where(e => e.TransactionID == giftID && e.Giver.PersonGUID == _userId)
                        .Select(
                            e =>
                                new TransactionListItem
                                {
                                    TransactionID = e.TransactionID,
                                    DateCreated = e.DateCreated,
                                    DateModified = e.DateModified,
                                    QtyGiven = e.QtyGiven,
                                    GiftID = e.GiftID,
                                    Gift = e.Gift,
                                    GiverID = e.GiverID,
                                    Giver = e.Giver,
                                    RecipientName = e.Gift.WishList.Owner.FirstName + " " + e.Gift.WishList.Owner.LastName
                                }
                        );

                return query.ToArray();
            }
        }

        public IEnumerable<TransactionListItem> GetAllTransactionsForUser()
        {
            using (var ctx = new ApplicationDbContext())
            {
                var query =
                    ctx
                        .Transactions
                        .Include("Giver")
                        .Include("Gift")
                        .Include("Gift.WishList")
                        .Include("Gift.WishList.Owner")
                        .Where(e => e.Giver.PersonGUID == _userId)
                        .Select(
                            e =>
                                new TransactionListItem
                                {
                                    TransactionID = e.TransactionID,
                                    DateCreated = e.DateCreated,
                                    DateModified = e.DateModified,
                                    QtyGiven = e.QtyGiven,
                                    GiftID = e.GiftID,
                                    Gift = e.Gift,
                                    GiverID = e.GiverID,
                                    Giver = e.Giver,
                                    RecipientName = e.Gift.WishList.Owner.FirstName + " " + e.Gift.WishList.Owner.LastName
                                }
                        );

                return query.ToArray();
            }
        }

        public TransactionDetail GetTransactionByID(int id)
        {
            using (var ctx = new ApplicationDbContext())
            {
                if(ctx.Transactions.Any(e => e.TransactionID == id))
                {
                    var entity =
                    ctx
                        .Transactions
                        .Include("Giver")
                        .Include("Gift")
                        .Include("Gift.WishList")
                        .Include("Gift.WishList.Owner")
                        .Single(e => e.TransactionID == id);

                    return
                        new TransactionDetail
                        {
                            TransactionID = entity.TransactionID,
                            DateCreated = entity.DateCreated,
                            DateModified = entity.DateModified,
                            QtyGiven = entity.QtyGiven,
                            GiftID = entity.GiftID,
                            Gift = entity.Gift,
                            GiverID = entity.GiverID,
                            Giver = entity.Giver,
                            RecipientName = entity.Gift.WishList.Owner.FirstName + " " + entity.Gift.WishList.Owner.LastName
                        };
                }

                return null;
            }
        }

        public bool UpdateTransaction(TransactionEdit model)
        {
            using (var ctx = new ApplicationDbContext())
            {
                var entity =
                    ctx
                        .Transactions
                        .Single(e => e.TransactionID == model.TransactionID && e.Giver.PersonGUID == _userId);

                entity.DateCreated = model.DateCreated;
                entity.DateModified = DateTime.Now;
                entity.QtyGiven = model.QtyGiven;
                entity.GiftID = model.GiftID;
                entity.GiverID = model.GiverID;

                return ctx.SaveChanges() == 1;
            }
        }

        public bool DeleteTransaction(int id)
        {
            using (var ctx = new ApplicationDbContext())
            {
                var entity =
                    ctx
                        .Transactions
                        .Include("Gift")
                        .Include("Gift.WishList")
                        .Include("Gift.WishList.Owner")
                        .Single(e => e.TransactionID == id && (e.Giver.PersonGUID == _userId || e.Gift.WishList.OwnerGUID == _userId));

                // Before deleting transaction, we want to notify buyers that their transaction was deleted
                var service = CreateNotificationService();
                NotificationDetail model = new NotificationDetail()
                {
                    NotificationType = NotificationType.ReadOnlyNegative,
                    Message = $"Your transaction for {entity.Gift.Name} (qty. {entity.QtyGiven}) was cancelled. {entity.Gift.WishList.Owner.FullName} removed this item from their wish list.",
                    RecipientID = (int)entity.GiverID,
                    SenderID = (int)entity.Gift.WishList.OwnerID
                };
                service.CreateNotification(model);

                ctx.Transactions.Remove(entity);

                return ctx.SaveChanges() == 1;
            }
        }

        public int GetCurrentUserID()
        {
            using (var ctx = new ApplicationDbContext())
            {
                if (ctx.People.Any(p => p.PersonGUID == _userId))
                {
                    var user = ctx.People.Single(p => p.PersonGUID == _userId);
                    return user.PersonID;
                }

                return -1;
            }
        }

        private NotificationService CreateNotificationService()
        {
            var service = new NotificationService(_userId);
            return service;
        }
    }
}
