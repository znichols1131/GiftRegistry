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
        public DateTime? Birthdate { get; set; }

        [Display(Name = "Profile Picture")]
        public byte[] ProfilePicture { get; set; }

        public virtual ICollection<WishList> WishLists { get; set; } = new List<WishList>();
        public virtual ICollection<Friend> Friends { get; set; } = new List<Friend>();
        public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();

    }
}
