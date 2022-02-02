using GiftRegistry.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GiftRegistry.Models
{
    public class GiftEdit
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

        [Required]
        public int WishListID { get; set; }
        public WishList WishList { get; set; }

        [Display(Name = "Product Image")]
        public ImageModel Image { get; set; }
        public int ImageID { get; set; }
    }
}
