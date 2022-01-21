using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GiftRegistry.Data
{
    public class Friend
    {
        [Key]
        public int FriendID { get; set; }

        [Required]
        public Guid OwnerGUID { get; set; }

        [Display(Name = "Relationship")]
        [MinLength(2, ErrorMessage = "Please enter at least 2 characters.")]
        public string Relationship { get; set; }

        [Required]
        [ForeignKey(nameof(Person))]
        public int PersonID { get; set; }
        public virtual Person Person { get; set; }
    }
}
