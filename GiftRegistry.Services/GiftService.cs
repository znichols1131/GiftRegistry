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
                    SourceURL = GetClickableLink(model.SourceURL),
                    QtyDesired = model.QtyDesired,
                    WishListID = model.WishListID,
                    ProductImage = model.Image.ImageData
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
                        Image = (entity.ProductImage == null || entity.ProductImage.Length == 0) ? CreateDefaultImageModel() : CreateImageModelForBytes(entity.ProductImage)
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
                entity.SourceURL = GetClickableLink(model.SourceURL);
                entity.QtyDesired = model.QtyDesired;
                entity.ProductImage = model.Image.ImageData;

                return ctx.SaveChanges() == 1;
            }
        }

        public bool DeleteGift(int id)
        {
            List<Transaction> transactions;
            using (var ctx = new ApplicationDbContext())
            {
                var entity =
                    ctx
                        .Gifts
                        .Include("Transactions")
                        .Single(e => e.GiftID == id && e.WishList.OwnerGUID == _userId);

                transactions = entity.Transactions.ToList();
            }

            // Don't rely on cascade deleting. We want to send notification to buyers whose transactions will be deleted
            var transactionService = CreateTransactionService();
            foreach (var transaction in transactions)
            {
                transactionService.DeleteTransaction(transaction.TransactionID);
            }

            using (var ctx = new ApplicationDbContext())
            {
                var entity =
                    ctx
                        .Gifts
                        .Include("Transactions")
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

        private string GetClickableLink(string url)
        {
            if (url.StartsWith("https://") || url.StartsWith("http://") || url.StartsWith("//"))
                return url;

            return "http://" + url;
        }




        private ImageService CreateImageService()
        {
            var service = new ImageService(_userId);
            return service;
        }

        private ImageModel CreateImageModelForBytes(byte[] input)
        {
            var service = CreateImageService();

            service.DeleteImagesForUser();

            var model = new ImageModel();
            model.ImageData = input;
            model.OwnerGUID = _userId;

            if (!service.CreateImage(model))
                return null;

            return service.GetLatestImageForUser();
        }

        private ImageModel CreateDefaultImageModel()
        {
            var service = CreateImageService();

            service.DeleteImagesForUser();

            if (!service.CreateDefaultImage(false))
                return null;

            var image = service.GetLatestImageForUser();

            return image;
        }

        private TransactionService CreateTransactionService()
        {
            var service = new TransactionService(_userId);
            return service;
        }
    }
}
