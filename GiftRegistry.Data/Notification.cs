using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GiftRegistry.Data
{
    public class Notification
    {
        [Key]
        public int NotificationID { get; set; }

        [Required]
        [Display(Name = "Type")]
        public NotificationType NotificationType { get; set; }

        [Required]
        [Display(Name = "Message")]
        public string Message { get; set; }

        [Required]
        [Display(Name = "Date Created")]
        [DataType(DataType.Date)]
        public DateTime DateCreated { get; set; }

        [Display(Name = "Date Updated")]
        [DataType(DataType.Date)]
        public DateTime? DateUpdated { get; set; }

        [Required]
        [ForeignKey(nameof(Recipient))]
        [Display(Name = "Recipient")]
        public int RecipientID { get; set; }
        public Person Recipient { get; set; }

        [ForeignKey(nameof(Sender))]
        [Display(Name = "Sender")]
        public int? SenderID { get; set; }
        public Person Sender { get; set; }
    }

    public enum NotificationType
    {
        ReadOnlyMessage = 0,
        FriendRequest = 1,
        ReadOnlyNegative = 2,
        ReadOnlyPositive = 3
    }
}
