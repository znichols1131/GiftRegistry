using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GiftRegistry.Models
{
    public class WishListListItem
    {
        public int WishListID { get; set; }
        public Guid OwnerGUID { get; set; }

        [Display(Name = "List Name")]
        public string Name { get; set; }

        [Display(Name = "Due Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MMM d, yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? DueDate { get; set; }

        public int OwnerID { get; set; }


        [Display(Name = "Gift Count")]
        public int GiftCount { get; set; }
    }
}
