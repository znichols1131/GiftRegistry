using GiftRegistry.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GiftRegistry.Models
{
    public class WishListDetail
    {
        public int WishListID { get; set; }
        public Guid OwnerGUID { get; set; }

        [Display(Name = "List Name")]
        public string Name { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        [Display(Name = "Due Date")]
        [DataType(DataType.Date)]
        public DateTime? DueDate { get; set; }

        [Display(Name = "List Owner")]
        public string OwnerName { get; set; }
        public int OwnerID { get; set; }

        public List<Gift> Gifts { get; set; }
    }
}
