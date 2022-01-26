using GiftRegistry.Data;
using GiftRegistry.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GiftRegistry.Services
{
    public class HomeService
    {
        private readonly Guid _userId;

        public HomeService(Guid userId)
        {
            _userId = userId;
        }

        public IEnumerable<EventListItem> GetEvents()
        {
            using (var ctx = new ApplicationDbContext())
            {
                List<EventListItem> events = new List<EventListItem>();
                DateTime now = DateTime.Now;

                var friends =
                    ctx
                        .Friends
                        .Include("Person")
                        .Where(e => e.OwnerGUID == _userId)
                        .Select(e =>
                                new
                                {
                                    OwnerName = e.Person.FirstName + " " + e.Person.LastName,
                                    OwnerID = e.PersonID,
                                    WishLists = e.Person.WishLists
                                }).ToList();

                foreach(var friend in friends)
                {
                    foreach(var wishList in friend.WishLists)
                    {
                        EventListItem newEvent = new EventListItem();

                        newEvent.OwnerID = friend.OwnerID;
                        newEvent.OwnerName = friend.OwnerName;
                        newEvent.WishListID = wishList.WishListID;
                        newEvent.EventName = wishList.Name;
                        newEvent.EventDate = (DateTime)wishList.DueDate;                        
                        newEvent.DaysRemaining = (int)((TimeSpan)((DateTime)wishList.DueDate - now)).TotalDays;

                        events.Add(newEvent);
                    }
                }

                //var friends =
                //    ctx
                //        .Friends
                //        .Include("Person")
                //        .Where(e => e.OwnerGUID == _userId);                        

                //foreach (var friend in friends)
                //{
                //    var person = ctx.Entry(friend).Entity.Person;
                //    var wishLists = ctx.Entry(person).Entity.WishLists.ToList();
                //    foreach(var wishList in wishLists)
                //    {
                //        EventListItem newEvent = new EventListItem();

                //        newEvent.OwnerID = person.PersonID;
                //        newEvent.OwnerName = person.FullName;
                //        newEvent.WishListID = wishList.WishListID;
                //        newEvent.EventName = wishList.Name;
                //        newEvent.EventDate = (DateTime)wishList.DueDate;

                //        events.Add(newEvent);
                //    }
                //}

                return events.OrderBy(e => e.EventDate);
            }
        }
    }
}
