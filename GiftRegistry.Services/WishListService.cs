using GiftRegistry.Data;
using GiftRegistry.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GiftRegistry.Services
{
    public class WishListService
    {
        private readonly Guid _userId;
        private readonly int _userOwnerID;

        public WishListService(Guid userId)
        {
            _userId = userId;
            _userOwnerID = GetCurrentUserID();
        }

        public bool CreateWishList(WishListCreate model)
        {
            var entity =
                new WishList()
                {
                    OwnerID = _userOwnerID,
                    OwnerGUID = _userId,
                    Name = model.Name,
                    Description = model.Description,
                    DueDate = model.DueDate
                };

            using (var ctx = new ApplicationDbContext())
            {
                ctx.WishLists.Add(entity);
                return ctx.SaveChanges() == 1;
            }
        }

        public IEnumerable<WishListListItem> GetWishListsForCurrentUser()
        {
            using (var ctx = new ApplicationDbContext())
            {
                var query =
                    ctx
                        .WishLists
                        .Where(e => e.OwnerGUID == _userId)
                        .Select(
                            e =>
                                new WishListListItem
                                {
                                    WishListID = e.WishListID,
                                    OwnerID = e.OwnerID,
                                    OwnerGUID = e.OwnerGUID,
                                    Name = e.Name,
                                    DueDate = e.DueDate,
                                    GiftCount = e.Gifts.Count
                                }
                        );

                return query.ToArray();
            }
        }

        public IEnumerable<WishListListItem> GetWishListsForOwnerID(int ownerID)
        {
            using (var ctx = new ApplicationDbContext())
            {
                var query =
                    ctx
                        .WishLists
                        .Where(e => e.OwnerID == ownerID)
                        .Select(
                            e =>
                                new WishListListItem
                                {
                                    WishListID = e.WishListID,
                                    OwnerID = e.OwnerID,
                                    OwnerGUID = e.OwnerGUID,
                                    Name = e.Name,
                                    DueDate = e.DueDate,
                                    GiftCount = e.Gifts.Count
                                }
                        );

                return query.ToArray();
            }
        }

        public WishListDetail GetWishListByID(int id)
        {
            using (var ctx = new ApplicationDbContext())
            {
                var entity =
                    ctx
                        .WishLists
                        .Include("Owner")
                        .Single(e => e.WishListID == id);

                var result = new WishListDetail
                {
                    WishListID = entity.WishListID,
                    OwnerID = entity.OwnerID,
                    OwnerGUID = entity.OwnerGUID,
                    Name = entity.Name,
                    Description = entity.Description,
                    DueDate = entity.DueDate,
                    OwnerName = entity.Owner.FullName,
                    OwnerImage = entity.Owner.ProfilePicture,
                    Gifts = new List<GiftListItem>(),
                    GiftCount = entity.Gifts.Count
                };

                foreach (var gift in entity.Gifts)
                {
                    result.Gifts.Add(new GiftListItem
                    {
                        GiftID = gift.GiftID,
                        Name = gift.Name,
                        Description = gift.Description,
                        SourceURL = gift.SourceURL,
                        QtyDesired = gift.QtyDesired,
                        WishListID = gift.WishListID,
                        WishList = gift.WishList,
                        ProductImage = gift.ProductImage
                    });
                }

                return result;
            }
        }

        public bool UpdateWishList(WishListEdit model)
        {
            using (var ctx = new ApplicationDbContext())
            {
                var entity =
                    ctx
                        .WishLists
                        .Single(e => e.WishListID == model.WishListID && e.OwnerGUID == _userId);

                entity.Name = model.Name;
                entity.Description = model.Description;
                entity.DueDate = model.DueDate;

                return ctx.SaveChanges() == 1;
            }
        }

        public bool DeleteWishList(int id)
        {
            using (var ctx = new ApplicationDbContext())
            {
                var entity =
                    ctx
                        .WishLists
                        .Include("Gifts")
                        .Single(e => e.WishListID == id && e.OwnerGUID == _userId);

                // Don't rely on cascade deleting. We want to send notification to buyers whose transactions will be deleted
                var giftService = CreateGiftService();
                foreach (var gift in entity.Gifts)
                {
                    giftService.DeleteGift(gift.GiftID);
                }

                ctx.WishLists.Remove(entity);

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
        private GiftService CreateGiftService()
        {
            var service = new GiftService(_userId);
            return service;
        }
    }
}
