using GiftRegistry.Data;
using GiftRegistry.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GiftRegistry.Services
{
    public class GiftService
    {
        private readonly Guid _userId;

        public GiftService(Guid userId)
        {
            _userId = userId;
        }

        public bool CreateGift(GiftCreate model)
        {
            var entity =
                new Gift()
                {
                    Name = model.Name,
                    Description = model.Description,
                    SourceURL = model.SourceURL,
                    QtyDesired = model.QtyDesired,
                    WishListID = model.WishListID,
                    ProductImage = model.ProductImage
                };

            using (var ctx = new ApplicationDbContext())
            {
                ctx.Gifts.Add(entity);
                return ctx.SaveChanges() == 1;
            }
        }

        public IEnumerable<GiftListItem> GetGiftsForWishListID(int wishListID)
        {
            using (var ctx = new ApplicationDbContext())
            {
                var query =
                    ctx
                        .Gifts
                        .Where(e => e.WishListID == wishListID)
                        .Select(
                            e =>
                                new GiftListItem
                                {
                                    GiftID = e.GiftID,
                                    Name = e.Name,
                                    Description = e.Description,
                                    QtyDesired = e.QtyDesired,
                                    WishListID = e.WishListID,
                                    WishList = e.WishList,
                                    ProductImage = e.ProductImage
                                }
                        );

                return query.ToArray();
            }
        }

        public GiftDetail GetGiftByID(int id)
        {
            using (var ctx = new ApplicationDbContext())
            {
                var entity =
                    ctx
                        .Gifts
                        .Include("WishList")
                        .Include("WishList.Owner")
                        .Single(e => e.GiftID == id);

                return
                    new GiftDetail
                    {
                        GiftID = entity.GiftID,
                        Name = entity.Name,
                        Description = entity.Description,
                        SourceURL = entity.SourceURL,
                        QtyDesired = entity.QtyDesired,
                        QtyPurchased = QuantityPurchasedForGiftID(entity.GiftID),
                        WishListID = entity.WishListID,
                        WishList = entity.WishList,
                        ProductImage = entity.ProductImage
                    };
            }
        }

        public bool UpdateGift(GiftEdit model)
        {
            using (var ctx = new ApplicationDbContext())
            {
                var entity =
                    ctx
                        .Gifts
                        .Single(e => e.GiftID == model.GiftID && e.WishList.OwnerGUID == _userId);

                entity.Name = model.Name;
                entity.Description = model.Description;
                entity.SourceURL = model.SourceURL;
                entity.QtyDesired = model.QtyDesired;
                entity.ProductImage = model.ProductImage;

                return ctx.SaveChanges() == 1;
            }
        }

        public bool DeleteGift(int id)
        {
            using (var ctx = new ApplicationDbContext())
            {
                var entity =
                    ctx
                        .Gifts
                        .Single(e => e.GiftID == id && e.WishList.OwnerGUID == _userId);

                ctx.Gifts.Remove(entity);

                return ctx.SaveChanges() == 1;
            }
        }

        public int QuantityPurchasedForGiftID(int giftID)
        {
            using (var ctx = new ApplicationDbContext())
            {
                var query =
                    ctx
                        .Transactions
                        .Where(e => e.GiftID == giftID);

                if (query.Count() == 0)
                    return 0;

                return query.Sum(t => t.QtyGiven);
            }
        }
    }
}
