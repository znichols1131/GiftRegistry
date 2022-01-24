using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GiftRegistry.Data
{
    public class Person
    {
        [Key]
        public int PersonID { get; set; }

        [Required]
        public Guid PersonGUID { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Display(Name = "Full Name")]
        public string FullName { get { return FirstName + " " + LastName; } }

        [Display(Name = "Birthdate")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MMM dd, yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? Birthdate { get; set; }

        public virtual ICollection<WishList> WishLists { get; set; } = new List<WishList>();
        public virtual ICollection<Friend> Friends { get; set; } = new List<Friend>();
        public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();

    }
}
