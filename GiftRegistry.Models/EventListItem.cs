using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GiftRegistry.Models
{
    public class EventListItem
    {
        public int OwnerID { get; set; }

        [Display(Name = "Friend")]
        public string OwnerName { get; set; }

        public int WishListID { get; set; }

        [Display(Name = "Event Name")]
        public string EventName { get; set; }

        [Display(Name = "Event Date")]
        [DataType(DataType.Date)]
        public DateTime EventDate { get; set; }

        [Display(Name = "Days Remaining")]
        public int DaysRemaining { get; set; }
    }
}
