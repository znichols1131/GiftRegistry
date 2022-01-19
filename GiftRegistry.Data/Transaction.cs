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
        public DateTime DateCreated { get; set; }

        [Display(Name = "Modified")]
        public DateTime? DateModified { get; set; }

        [Required]
        [Display(Name = "Qty. Given")]
        [Range(0, int.MaxValue, ErrorMessage ="The quantity must be 0 or more.")]
        public int QtyGiven { get; set; }

        [Required]
        [ForeignKey(nameof(Gift))]
        public int GiftID { get; set; }
        public Gift Gift { get; set; }

        [Required]
        [ForeignKey(nameof(Giver))]
        public int GiverID { get; set; }
        public Person Giver { get; set; }

    }
}
