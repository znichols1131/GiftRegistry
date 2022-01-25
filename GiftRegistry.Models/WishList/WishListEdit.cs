using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GiftRegistry.Models
{
    public class WishListEdit
    {
        [Key]
        public int WishListID { get; set; }

        [Required]
        public Guid OwnerGUID { get; set; }

        [Required]
        [Display(Name = "List Name")]
        [MinLength(2, ErrorMessage = "Please enter at least 2 characters.")]
        [MaxLength(50, ErrorMessage = "There are too many characters in this field.")]
        public string Name { get; set; }

        [Display(Name = "Description")]
        [MaxLength(100, ErrorMessage = "There are too many characters in this field.")]
        public string Description { get; set; }

        [Display(Name = "Due Date")]
        [DataType(DataType.Date)]
        public DateTime? DueDate { get; set; }

        [Required]
        [Display(Name = "List Owner")]
        public int OwnerID { get; set; }
    }
}
