using GiftRegistry.Data;
using GiftRegistry.Services;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GiftRegistry.WebMVC.Controllers
{
    public class UserRoleController : Controller
    {
        // GET: UserRole
        [Authorize]
        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            var service = CreateUserRoleService();
            var model = service.GetUsers();

            foreach(var user in model)
            {
                if(string.IsNullOrWhiteSpace(user.UserRoleName))
                {
                    try
                    {
                        var _userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
                        string[] roles = _userManager.GetRoles(user.UserGUID.ToString()).ToArray();
                        if (roles != null && roles.Length > 0)
                        {
                            string role = roles.First();
                            if (!string.IsNullOrWhiteSpace(role))
                            {
                                user.UserRoleName = role;
                            }
                        }
                    }
                    catch { }                    
                }
            }

            return View(model);
        }

        //[HttpGet]
        //public ActionResult Create()
        //{
        //    var role = new IdentityRole();
        //    return View(role);
        //}

        //[HttpPost]
        //public ActionResult Create(IdentityRole role)
        //{
        //    using (var ctx = new ApplicationDbContext())
        //    {
        //        ctx.Roles.Add(role);

        //        if (ctx.SaveChanges() > 0)
        //        {
        //            return RedirectToAction("Index");
        //        }

        //        return View(role);
        //    }
        //}

        private UserRoleService CreateUserRoleService()
        {
            var userId = Guid.Parse(User.Identity.GetUserId());
            var service = new UserRoleService(userId);
            return service;
        }
    }
}