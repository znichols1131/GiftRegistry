using GiftRegistry.Data;
using GiftRegistry.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GiftRegistry.Services
{
    public class UserRoleService
    {
        private readonly Guid _userId;

        public UserRoleService(Guid userId)
        {
            _userId = userId;
        }

        public IEnumerable<UserRoleDetail> GetUsers()
        {
            using (var ctx = new ApplicationDbContext())
            {
                if(ctx.Users.Any())
                {
                    var users = ctx.Users.ToArray();

                    List<UserRoleDetail> result = new List<UserRoleDetail>();
                    foreach (var user in users)
                    {
                        result.Add(GetInfoForUser(user));
                    }

                    return result.OrderBy(e => e.FullName);
                }                
            }

            return null;
        }

        public IEnumerable<UserRoleDetail> GetUsersForSearch(string search)
        {
            using (var ctx = new ApplicationDbContext())
            {
                if (ctx.Users.Any())
                {
                    var users = ctx.Users.ToArray();

                    List<UserRoleDetail> result = new List<UserRoleDetail>();
                    foreach (var user in users)
                    {
                        result.Add(GetInfoForUser(user));
                    }

                    return result.Where(e => e.FullName.ToLower().Contains(search.ToLower())).OrderBy(e => e.FullName);
                }
            }

            return null;
        }

        public UserRoleDetail GetUserForGuid(Guid guid)
        {
            using (var ctx = new ApplicationDbContext())
            {
                if(ctx.Users.Any(e => e.Id == guid.ToString()))
                {
                    var user =
                    ctx
                        .Users
                        .Where(e => e.Id == guid.ToString())
                        .FirstOrDefault();

                    return GetInfoForUser(user);
                }                
            }

            return null;
        }

        public IEnumerable<IdentityRole> GetAllRoles()
        {
            using (var ctx = new ApplicationDbContext())
            {
                return ctx.Roles.ToArray();
            }
        }

        //public bool UpdateUserRole(UserRoleDetail model)
        //{
        //    using (var ctx = new ApplicationDbContext())
        //    {
        //        var user = ctx.Users.Where(i => Guid.Parse(i.Id) == model.UserGUID).FirstOrDefault();
                

        //        var userId = ctx.Users.Where(i => i.UserName == user.UserName).Select(s => s.Id);
        //        string updateId = "";
        //        foreach (var i in userId)
        //        {
        //            updateId = i.ToString();
        //        }
        //        // Assign role to user here
        //        this.UserManager.AddToRoleAsync(updateId, model.UserRoleName);

        //        return true;
        //    }            
        //}

        private UserRoleDetail GetInfoForUser(ApplicationUser user)
        {
            UserRoleDetail result = new UserRoleDetail();
            result.UserGUID = Guid.Parse(user.Id);
            if(user.Roles != null && user.Roles.Count > 0)
            {
                result.UserRoleName = user.Roles.FirstOrDefault().ToString();
            }else
            {
                result.UserRoleName = "";
            }

            var service = CreatePersonService();
            PersonDetail person = service.GetPersonByGUID(result.UserGUID);

            result.PersonID = person.PersonID;
            result.FullName = person.FirstName + " " + person.LastName;

            return result;
        }

        private PersonService CreatePersonService()
        {
            var service = new PersonService(_userId);
            return service;
        }
    }
}
