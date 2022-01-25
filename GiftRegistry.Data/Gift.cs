using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GiftRegistry.Data
{
    public class Gift
    {
        [Key]
        public int GiftID { get; set; }

        [Required]
        [Display(Name = "Gift Name")]
        public string Name { get; set; }

        [Display(Name = "Description")]
        [MaxLength(100, ErrorMessage = "There are too many characters in this field.")]
        public string Description { get; set; }

        [Display(Name = "Link")]
        public string SourceURL { get; set; }

        [Display(Name = "Qty. Desired")]
        [Range(0, int.MaxValue, ErrorMessage = "The quantity must be 0 or more.")]
        public int QtyDesired { get; set; }

        [Display(Name = "Qty. Purchased")]
        [Range(0, int.MaxValue, ErrorMessage = "The quantity must be 0 or more.")]
        public int QtyPurchased 
        {
            get
            {
                return 0;
            }
        }

        [Required]
        [ForeignKey(nameof(WishList))]
        public int WishListID { get; set; }
        public virtual WishList WishList { get; set; }

        public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();

    }
}
