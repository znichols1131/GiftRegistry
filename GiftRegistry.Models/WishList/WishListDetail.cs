using GiftRegistry.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GiftRegistry.Models
{
    public class WishListDetail
    {
        public int WishListID { get; set; }
        public Guid OwnerGUID { get; set; }

        [Display(Name = "List Name")]
        public string Name { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        [Display(Name = "Due Date")]
        [DataType(DataType.Date)]
        public DateTime? DueDate { get; set; }

        [Display(Name = "List Owner")]
        public string OwnerName { get; set; }
        public int OwnerID { get; set; }

        [Display(Name = "Gift Count")]
        public int GiftCount { get; set; }

        [Display(Name = "List Owner's Picture")]
        public byte[] OwnerImage { get; set; }

        public List<GiftListItem> Gifts { get; set; }

        public int QtyPurchasedForGiftID(int giftID)
        {
            using (var ctx = new ApplicationDbContext())
            {
                var query =
                    ctx
                        .Transactions
                        .Where(e => e.GiftID == giftID)
                        .Select(
                            e =>
                                new 
                                {
                                    QtyGiven = e.QtyGiven
                                }
                        );

                if (query.Count() == 0)
                    return 0;

                return query.Sum(t => t.QtyGiven);
            }
        }
    }
}
