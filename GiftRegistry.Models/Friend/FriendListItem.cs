using GiftRegistry.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GiftRegistry.Models
{
    public class FriendListItem
    {
        public int FriendID { get; set; }

        public Guid OwnerGUID { get; set; }


        [Display(Name = "Relationship")]
        public string Relationship { get; set; }

        public int PersonID { get; set; }
        public virtual Person Person { get; set; }
    }
}
