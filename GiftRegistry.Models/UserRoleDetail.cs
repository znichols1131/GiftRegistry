using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GiftRegistry.Models
{
    public class UserRoleDetail
    {
        public Guid UserGUID { get; set; }

        [Display(Name = "User Role")]
        public string UserRoleName { get; set; }

        public int PersonID { get; set; }

        [Display(Name = "User Name")]
        public string FullName { get; set; }
    }
}
