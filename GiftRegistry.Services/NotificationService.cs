﻿using GiftRegistry.Data;
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
                        );

                return query.ToArray();
            }
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
                var entity =
                    ctx
                        .Notifications
                        .Single(e => e.NotificationID == id && (e.Recipient.PersonGUID == _userId));

                ctx.Notifications.Remove(entity);

                return ctx.SaveChanges() == 1;
            }
        }
    }
}