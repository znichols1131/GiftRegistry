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
                    Relationship = model.Relationship,
                    PersonID = model.PersonID
                };

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
                        .Select(
                            e =>
                                new FriendListItem
                                {
                                    FriendID = e.FriendID,
                                    OwnerGUID = e.OwnerGUID,
                                    Relationship = e.Relationship,
                                    PersonID = e.PersonID,
                                    Person = e.Person
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
                        .Single(e => e.FriendID == id && e.OwnerGUID == _userId);

                return
                    new FriendDetail
                    {
                        FriendID = entity.FriendID,
                        OwnerGUID = entity.OwnerGUID,
                        Relationship = entity.Relationship,
                        PersonID = entity.PersonID,
                        Person = entity.Person
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
                        .Single(e => e.FriendID == model.FriendID && e.OwnerGUID == _userId);

                entity.Relationship = model.Relationship;

                return ctx.SaveChanges() == 1;
            }
        }

        public bool DeleteFriend(int id)
        {
            using (var ctx = new ApplicationDbContext())
            {
                var entity =
                    ctx
                        .Friends
                        .Single(e => e.FriendID == id && e.OwnerGUID == _userId);

                ctx.Friends.Remove(entity);

                return ctx.SaveChanges() == 1;
            }
        }
    }
}
