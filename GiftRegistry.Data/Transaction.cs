using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GiftRegistry.Data
{
    public class Transaction
    {
        [Key]
        public int TransactionID { get; set; }

        [Display(Name = "Created")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MMM dd, yyyy}", ApplyFormatInEditMode = true)] 
        public DateTime DateCreated { get; set; }

        [Display(Name = "Modified")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MMM dd, yyyy}", ApplyFormatInEditMode = true)] 
        public DateTime? DateModified { get; set; }

        [Required]
        [Display(Name = "Qty. Given")]
        [Range(0, int.MaxValue, ErrorMessage ="The quantity must be 0 or more.")]
        public int QtyGiven { get; set; }

        [Required]
        [ForeignKey(nameof(Gift))]
        public int? GiftID { get; set; }        // Nullable to prevent gift from being deleted (if Giver is deleted, transaction will be deleted but not gift)   
        public virtual Gift Gift { get; set; }

        [ForeignKey(nameof(Giver))]
        public int? GiverID { get; set; }       // Nullable to prevent giver from being deleted (if Gift is deleted, transaction will be deleted but not giver)
        public virtual Person Giver { get; set; }

    }
}
