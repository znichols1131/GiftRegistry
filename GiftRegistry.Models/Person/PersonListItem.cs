using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GiftRegistry.Models
{
    public class PersonListItem
    {
        public int PersonID { get; set; }

        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        [Display(Name = "Birthdate")]
        public DateTime? Birthdate { get; set; }

    }
}
