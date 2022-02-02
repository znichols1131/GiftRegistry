using GiftRegistry.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GiftRegistry.Models
{
    public class NotificationListItem
    {
        public int NotificationID { get; set; }

        [Display(Name = "Type")]
        public NotificationType NotificationType { get; set; }

        [Display(Name = "Message")]
        public string Message { get; set; }

        [Display(Name = "Date Created")]
        public DateTime DateCreated { get; set; }

        [Display(Name = "Date Updated")]
        public DateTime? DateUpdated { get; set; }

        [Display(Name = "Recipient")]
        public int RecipientID { get; set; }
        public Person Recipient { get; set; }

        [Display(Name = "Sender")]
        public int? SenderID { get; set; }
        public Person Sender { get; set; }
    }
}
