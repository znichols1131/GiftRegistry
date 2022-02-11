using GiftRegistry.Data;
using GiftRegistry.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GiftRegistry.Services
{
    public class NotificationService
    {
        private readonly Guid _userId;

        public NotificationService(Guid userId)
        {
            _userId = userId;
        }

        public bool CreateNotification(NotificationDetail model)
        {
            var entity =
                new Notification()
                {
                    NotificationType = model.NotificationType,
                    Message = model.Message,
                    RecipientID = model.RecipientID,
                    SenderID = model.SenderID,
                    DateCreated = DateTime.Now
                };

            using (var ctx = new ApplicationDbContext())
            {
                ctx.Notifications.Add(entity);
                return ctx.SaveChanges() == 1;
            }
        }

        public IEnumerable<NotificationListItem> GetNotificationsForUser()
        {
            using (var ctx = new ApplicationDbContext())
            {
                var query =
                    ctx
                        .Notifications
                        .Include("Recipient")
                        .Include("Sender")
                        .Where(e => e.Recipient.PersonGUID == _userId)
                        .Select(
                            e =>
                                new NotificationListItem
                                {
                                    NotificationID = e.NotificationID,
                                    NotificationType = e.NotificationType,
                                    Message = e.Message,
                                    DateCreated = e.DateCreated,
                                    DateUpdated = e.DateUpdated,
                                    RecipientID = e.RecipientID,
                                    Recipient = e.Recipient,
                                    SenderID = e.SenderID,
                                    Sender = e.Sender
                                }
                        )
                        .OrderByDescending(e => e.DateUpdated == null ? e.DateCreated : e.DateUpdated);

                return query.ToArray();
            }
        }

        public int GetNotificationCountForUser()
        {
            using (var ctx = new ApplicationDbContext())
            {
                if(ctx.Notifications.Include("Recipient").Any(e => e.Recipient.PersonGUID == _userId))
                {
                    return ctx
                                .Notifications
                                .Include("Recipient")
                                .Where(e => e.Recipient.PersonGUID == _userId)
                                .Count();
                }
            }

            return 0;
        }

        public NotificationDetail GetNotificationByID(int id)
        {
            using (var ctx = new ApplicationDbContext())
            {
                var entity =
                    ctx
                        .Notifications
                        .Include("Recipient")
                        .Include("Sender")
                        .Single(e => e.NotificationID == id);

                return
                    new NotificationDetail
                    {
                        NotificationID = entity.NotificationID,
                        NotificationType = entity.NotificationType,
                        Message = entity.Message,
                        DateCreated = entity.DateCreated,
                        DateUpdated = entity.DateUpdated,
                        RecipientID = entity.RecipientID,
                        Recipient = entity.Recipient,
                        SenderID = entity.SenderID,
                        Sender = entity.Sender
                    };
            }
        }

        public bool UpdateNotification(NotificationDetail model)
        {
            using (var ctx = new ApplicationDbContext())
            {
                var entity =
                    ctx
                        .Notifications
                        .Single(e => e.NotificationID == model.NotificationID && e.Recipient.PersonGUID == _userId);

                entity.NotificationType = model.NotificationType;
                entity.Message = model.Message;
                entity.DateUpdated = DateTime.Now;
                entity.RecipientID = model.RecipientID;
                entity.SenderID = model.SenderID;

                return ctx.SaveChanges() == 1;
            }
        }

        public bool DeleteNotification(int id)
        {
            using (var ctx = new ApplicationDbContext())
            {
                if(ctx.Notifications.Include("Recipient").Any(e => e.NotificationID == id && (e.Recipient.PersonGUID == _userId)))
                {
                    var entity =
                    ctx
                        .Notifications
                        .Include("Recipient")
                        .Single(e => e.NotificationID == id && (e.Recipient.PersonGUID == _userId));

                    ctx.Notifications.Remove(entity);

                    return ctx.SaveChanges() > 0;
                }
            }

            return false;
        }

        public bool DeleteNotificationForPendingFriend(FriendDetail friendship)
        {
            using (var ctx = new ApplicationDbContext())
            {
                if(ctx.Notifications.Any(e => e.NotificationType == NotificationType.FriendRequest && e.Sender.PersonGUID == friendship.OwnerGUID && e.RecipientID == friendship.PersonID))
                {
                    var entity = ctx
                                    .Notifications
                                    .Single(e => e.NotificationType == NotificationType.FriendRequest && 
                                                e.Sender.PersonGUID == friendship.OwnerGUID && 
                                                e.RecipientID == friendship.PersonID);

                    ctx.Notifications.Remove(entity);

                    return ctx.SaveChanges() == 1;
                }

                // Nothing to delete
                return true;
            }
        }

        public int AcceptFriendRequest(int notificationID)
        {
            // Get original friend request
            var originalRequest = GetNotificationByID(notificationID);

            // Fulfill original friend request
            var service = CreateFriendService();
            int friendID = service.AcceptFriendRequest(originalRequest.Sender, originalRequest.Recipient);
            if(friendID < 0) return -1;

            // Delete notification
            if (!DeleteNotification(notificationID)) return -1;

            // Send notification back to sender
            var newNotification = new NotificationDetail()
            {
                NotificationType = NotificationType.ReadOnlyMessage,
                Message = $"{originalRequest.Recipient.FullName} accepted your friend request.",
                Sender = originalRequest.Recipient,
                SenderID = originalRequest.RecipientID,
                Recipient = originalRequest.Sender,
                RecipientID = (int)originalRequest.SenderID
            };
            CreateNotification(newNotification);

            return friendID;
        }

        public void DenyFriendRequest(int notificationID)
        {
            // Get original friend request
            var originalRequest = GetNotificationByID(notificationID);

            // Deny original friend request
            var service = CreateFriendService();
            if (!service.DenyFriendRequest(originalRequest.Sender, originalRequest.Recipient)) return;

            // Delete notification
            DeleteNotification(notificationID);
        }

        private FriendService CreateFriendService()
        {
            var service = new FriendService(_userId);
            return service;
        }
    }
}
