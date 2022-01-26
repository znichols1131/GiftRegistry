using GiftRegistry.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GiftRegistry.Models
{
    public class TransactionEdit
    {
        [Key]
        public int TransactionID { get; set; }

        [Display(Name = "Created")]
        [DataType(DataType.Date)]
        public DateTime DateCreated { get; set; }

        [Display(Name = "Modified")]
        [DataType(DataType.Date)]
        public DateTime? DateModified { get; set; }

        [Required]
        [Display(Name = "Qty. Given")]
        [Range(0, int.MaxValue, ErrorMessage = "The quantity must be 0 or more.")]
        public int QtyGiven { get; set; }

        [Required]
        public int? GiftID { get; set; }        // Nullable to prevent gift from being deleted (if Giver is deleted, transaction will be deleted but not gift)   
        public Gift Gift { get; set; }
        public string GiftName { get; set; }
        public string RecipientName { get; set; }

        public int? GiverID { get; set; }       // Nullable to prevent giver from being deleted (if Gift is deleted, transaction will be deleted but not giver)
        public Person Giver { get; set; }
    }
}
