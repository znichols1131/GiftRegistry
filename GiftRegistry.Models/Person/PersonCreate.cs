using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GiftRegistry.Models
{
    public class PersonCreate
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Display(Name = "Birthdate")]
        [DataType(DataType.Date)]
        public DateTime? Birthdate { get; set; }
    }
}
