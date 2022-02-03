using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GiftRegistry.Models
{
    public class FriendEdit
    {
        [Required]
        public int FriendID { get; set; }

        [Required]
        public Guid OwnerGUID { get; set; }

        [Display(Name = "Relationship")]
        [MinLength(2, ErrorMessage = "Please enter at least 2 characters.")]
        public string Relationship { get; set; }

        [Display(Name = "Friend Request Is Pending")]
        public bool IsPending { get; set; }

        [Required]
        public int PersonID { get; set; }

        [Display(Name = "Friend Name")]
        public string PersonName { get; set; }
    }
}
