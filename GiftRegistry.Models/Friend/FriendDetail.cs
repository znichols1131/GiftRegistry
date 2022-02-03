using GiftRegistry.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GiftRegistry.Models
{
    public class FriendDetail
    {
        public int FriendID { get; set; }

        public Guid OwnerGUID { get; set; }


        [Display(Name = "Relationship")]
        public string Relationship { get; set; }

        [Display(Name = "Friend Request Is Pending")]
        public bool IsPending { get; set; }

        public int PersonID { get; set; }
        public Person Person { get; set; }

        [Display(Name = "Friend Name")]
        public string PersonName { get; set; }
    }
}
