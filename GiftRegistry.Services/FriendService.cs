using GiftRegistry.Data;
using GiftRegistry.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GiftRegistry.Services
{
    public class FriendService
    {
        private readonly Guid _userId;

        public FriendService(Guid userId)
        {
            _userId = userId;
        }

        public bool CreateFriend(FriendCreate model)
        {
            var entity =
                new Friend()
                {
                    OwnerGUID = _userId,
                    IsPending = model.IsPending,
                    Relationship = model.Relationship,
                    PersonID = model.PersonID
                };

            using (var ctx = new ApplicationDbContext())
            {
                ctx.Friends.Add(entity);
                return ctx.SaveChanges() == 1;
            }
        }

        public bool SendFriendRequest(FriendCreate model)
        {
            var entity =
                new Friend()
                {
                    OwnerGUID = _userId,
                    Relationship = model.Relationship,
                    PersonID = model.PersonID,
                    IsPending = true
                };

            var currentUser = GetCurrentUser();
            var notification = new NotificationDetail()
            {
                NotificationType = NotificationType.FriendRequest,
                Message = $"{currentUser.FirstName} {currentUser.LastName} sent you a friend request.",
                RecipientID = model.PersonID,
                SenderID = currentUser.PersonID
            };
            var service = CreateNotificationService();
            if (!service.CreateNotification(notification))
                return false;

            using (var ctx = new ApplicationDbContext())
            {
                ctx.Friends.Add(entity);
                return ctx.SaveChanges() == 1;
            }
        }

        public IEnumerable<FriendListItem> GetFriends()
        {
            using (var ctx = new ApplicationDbContext())
            {
                var query =
                    ctx
                        .Friends
                        .Include("Person")
                        .Where(e => e.OwnerGUID == _userId)
                        .OrderBy(f => f.Person.LastName)
                        .ThenBy(f => f.Person.FirstName)
                        .ThenBy(f => f.Person.Birthdate)
                        .Select(
                            e =>
                                new FriendListItem
                                {
                                    FriendID = e.FriendID,
                                    OwnerGUID = e.OwnerGUID,
                                    Relationship = e.Relationship,
                                    PersonID = e.PersonID,
                                    Person = e.Person,
                                    IsPending = e.IsPending
                                }
                        );

                return query.ToArray();
            }
        }

        public IEnumerable<FriendListItem> GetFriendsForSearchString(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return null;

            input = input.Trim();

            using (var ctx = new ApplicationDbContext())
            {
                var query =
                    ctx
                        .Friends
                        .Include("Person")
                        .Where(e => e.OwnerGUID == _userId && (e.Person.FirstName.ToLower().Contains(input.ToLower()) || e.Person.LastName.ToLower().Contains(input.ToLower())))
                        .OrderBy(f => f.Person.LastName)
                        .ThenBy(f => f.Person.FirstName)
                        .ThenBy(f => f.Person.Birthdate)
                        .Select(
                            e =>
                                new FriendListItem
                                {
                                    FriendID = e.FriendID,
                                    OwnerGUID = e.OwnerGUID,
                                    Relationship = e.Relationship,
                                    PersonID = e.PersonID,
                                    Person = e.Person,
                                    IsPending = e.IsPending
                                }
                        );

                return query.ToArray();
            }
        }

        public FriendDetail GetFriendByID(int id)
        {
            using (var ctx = new ApplicationDbContext())
            {
                var entity =
                    ctx
                        .Friends
                        .Include("Person")
                        .Include("Person.WishLists")
                        .Single(e => e.FriendID == id && (e.OwnerGUID == _userId || e.Person.PersonGUID == _userId));

                return
                    new FriendDetail
                    {
                        FriendID = entity.FriendID,
                        OwnerGUID = entity.OwnerGUID,
                        Relationship = entity.Relationship,
                        PersonID = entity.PersonID,
                        Person = entity.Person,
                        PersonName = entity.Person.FullName,
                        IsPending = entity.IsPending
                    };
            }
        }

        public bool UpdateFriend(FriendEdit model)
        {
            using (var ctx = new ApplicationDbContext())
            {
                var entity =
                    ctx
                        .Friends
                        .Single(e => e.FriendID == model.FriendID && (e.OwnerGUID == _userId || e.Person.PersonGUID == _userId));

                entity.Relationship = model.Relationship;

                return ctx.SaveChanges() == 1;
            }
        }

        public int AcceptFriendRequest(Person sender, Person recipient)
        {
            // Update sender's Friend model to no longer be pending
            using (var ctx = new ApplicationDbContext())
            {
                if (ctx.Friends.Any(e => e.OwnerGUID == sender.PersonGUID && e.PersonID == recipient.PersonID))
                {
                    var pendingFriend = ctx
                        .Friends
                        .Single(e => e.OwnerGUID == sender.PersonGUID && e.PersonID == recipient.PersonID);

                    pendingFriend.IsPending = false;

                    if (ctx.SaveChanges() == 0) return -1;
                }
            }

            // Create a new friend for Recipient and enter in the values from Sender
            FriendCreate createModel = new FriendCreate()
            {
                OwnerGUID = recipient.PersonGUID,
                PersonID = sender.PersonID,
                IsPending = false
            };
            if (!CreateFriend(createModel)) return -1;

            // Return new friend ID
            using (var ctx = new ApplicationDbContext())
            {
                if(ctx.Friends.Any(e => e.OwnerGUID == recipient.PersonGUID && e.PersonID == sender.PersonID))
                {
                    var entity = ctx
                                    .Friends
                                    .Single(e => e.OwnerGUID == recipient.PersonGUID && e.PersonID == sender.PersonID);

                    return entity.FriendID;
                }
            }

            return -1;
        }

        public bool DenyFriendRequest(Person sender, Person recipient)
        {
            // Delete the recipient from the sender's friends (if it was still pending)
            DeleteFriendship(sender.PersonGUID, recipient.PersonID);
            return true;
        }

        public bool DeleteFriend(int id)
        {
            // This method is called by PERSON A to delete a friendship with PERSON B.
            // However, we must delete the reciprocal friendship of PERSON B with PERSON A (if it exists and is not pending).

            FriendDetail friendship = GetFriendByID(id);

            // If friend was pending, delete notification before recipient can accept it
            if(friendship.IsPending)
            {
                var service = CreateNotificationService();
                service.DeleteNotificationForPendingFriend(friendship);
            }

            var personService = CreatePersonService();
            int personAID = personService.GetPersonByGUID(friendship.OwnerGUID).PersonID;

            // Delete reciprocal first (may not exist)
            DeleteFriendship(friendship.Person.PersonGUID, personAID);

            // Now delete the expected friendship
            return DeleteFriendship(friendship.OwnerGUID, friendship.PersonID);
        }

        public bool DeleteFriendship(Guid senderGUID, int recipientID)
        {
            using (var ctx = new ApplicationDbContext())
            {
                if(ctx.Friends.Any(e => e.OwnerGUID == senderGUID && e.PersonID == recipientID))
                {
                    var entity = ctx
                                    .Friends
                                    .Single(e => e.OwnerGUID == senderGUID && e.PersonID == recipientID);

                    ctx.Friends.Remove(entity);

                    return ctx.SaveChanges() == 1;
                }

                return false;
            }
        }

        private NotificationService CreateNotificationService()
        {
            var service = new NotificationService(_userId);
            return service;
        }

        private PersonService CreatePersonService()
        {
            var service = new PersonService(_userId);
            return service;
        }

        private PersonDetail GetCurrentUser()
        {
            using (var ctx = new ApplicationDbContext())
            {
                var entity =
                    ctx
                        .People
                        .Single(e => e.PersonGUID == _userId);

                return
                    new PersonDetail
                    {
                        PersonID = entity.PersonID,
                        FirstName = entity.FirstName,
                        LastName = entity.LastName,
                        Birthdate = entity.Birthdate,
                        Image = new ImageModel() { ImageData = entity.ProfilePicture }
                    };
            }
        }
    }
}
