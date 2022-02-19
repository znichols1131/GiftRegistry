using GiftRegistry.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GiftRegistry.Models
{
    public class TransactionListItem
    {
        public int TransactionID { get; set; }

        [Display(Name = "Created")]
        public DateTime DateCreated { get; set; }

        [Display(Name = "Modified")]
        public DateTime? DateModified { get; set; }

        [Display(Name = "Qty. Given")]
        public int QtyGiven { get; set; }

        public int? GiftID { get; set; }        // Nullable to prevent gift from being deleted (if Giver is deleted, transaction will be deleted but not gift)   
        public Gift Gift { get; set; }

        public int? GiverID { get; set; }       // Nullable to prevent giver from being deleted (if Gift is deleted, transaction will be deleted but not giver)
        public Person Giver { get; set; }

        public string RecipientName { get; set; }

        [DataType(DataType.Date)]
        public DateTime DateOfRecentActivity { get; set; }

    }
}
