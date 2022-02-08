using GiftRegistry.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GiftRegistry.Models
{
    public class GiftListItem
    {
        public int GiftID { get; set; }

        [Display(Name = "Gift Name")]
        public string Name { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        [Display(Name = "Link")]
        public string SourceURL { get; set; }

        [Display(Name = "Qty. Desired")]
        public int QtyDesired { get; set; }

        [Required]
        public int WishListID { get; set; }
        public WishList WishList { get; set; }

        [Display(Name = "Product Image")]
        public byte[] ProductImage { get; set; }
    }
}
