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
                DateTime today = DateTime.Now;

                var friends =
                    ctx
                        .Friends
                        .Include("Person")
                        .Where(e => e.OwnerGUID == _userId && !ctx.Friends.Any(f => f.OwnerGUID == _userId && f.PersonID == e.PersonID && f.IsPending))
                        .Select(e =>
                                new
                                {
                                    OwnerName = e.Person.FirstName + " " + e.Person.LastName,
                                    OwnerID = e.PersonID,
                                    WishLists = e.Person.WishLists,
                                    Birthday = e.Person.Birthdate
                                }).ToList();

                foreach(var friend in friends)
                {
                    // GET BIRTHDAY if it's within time frame

                    if(friend.Birthday != null)
                    {
                        DateTime nextBirthday = new DateTime(today.Year, ((DateTime)friend.Birthday).Month, ((DateTime)friend.Birthday).Day);
                        int nextAge = nextBirthday.Year - ((DateTime)friend.Birthday).Year;

                        if (nextBirthday.Month - today.Month <= 3)
                        {
                            // Birthday is within 3 months
                            EventListItem newBirthday = new EventListItem();

                            newBirthday.OwnerID = friend.OwnerID;
                            newBirthday.OwnerName = friend.OwnerName;
                            newBirthday.WishListID = -1;
                            newBirthday.EventName = string.Format($"{friend.OwnerName}'s Birthday (Age {nextAge})");
                            newBirthday.EventDate = nextBirthday;
                            newBirthday.DaysRemaining = (int)((TimeSpan)(nextBirthday - today)).TotalDays;

                            events.Add(newBirthday);
                        }
                    }

                    // GET WISH LISTS that fit time frame
                    foreach(var wishList in friend.WishLists)
                    {
                        if(wishList.DueDate != null)
                        {
                            EventListItem newEvent = new EventListItem();

                            newEvent.OwnerID = friend.OwnerID;
                            newEvent.OwnerName = friend.OwnerName;
                            newEvent.WishListID = wishList.WishListID;
                            newEvent.EventName = wishList.Name;
                            newEvent.EventDate = (DateTime)wishList.DueDate;
                            newEvent.DaysRemaining = (int)((TimeSpan)((DateTime)wishList.DueDate - today)).TotalDays;

                            events.Add(newEvent);
                        }                        
                    }
                }

                return events.OrderBy(e => e.EventDate);
            }
        }

        public IEnumerable<EventListItem> GetEventsWithinTime(string timeRange)
        {
            int monthRange = 3;
            if (!string.IsNullOrWhiteSpace(timeRange))
            {
                switch (timeRange)
                {
                    case "month01":
                        monthRange = 1;
                        break;
                    case "month03":
                        monthRange = 3;
                        break;
                    case "month06":
                        monthRange = 6;
                        break;
                    case "month12":
                        monthRange = 12;
                        break;
                    default:
                        monthRange = 3;
                        break;
                }
            }

            using (var ctx = new ApplicationDbContext())
            {
                List<EventListItem> events = new List<EventListItem>();
                DateTime today = DateTime.Now;

                var friends =
                    ctx
                        .Friends
                        .Include("Person")
                        .Where(e => e.OwnerGUID == _userId && !ctx.Friends.Any(f => f.OwnerGUID == _userId && f.PersonID == e.PersonID && f.IsPending))
                        .Select(e =>
                                new
                                {
                                    OwnerName = e.Person.FirstName + " " + e.Person.LastName,
                                    OwnerID = e.PersonID,
                                    WishLists = e.Person.WishLists,
                                    Birthday = e.Person.Birthdate
                                }).ToList();

                foreach (var friend in friends)
                {
                    if (friend.Birthday != null)
                    {
                        DateTime nextBirthday = new DateTime(today.Year, ((DateTime)friend.Birthday).Month, ((DateTime)friend.Birthday).Day);
                        int nextAge = nextBirthday.Year - ((DateTime)friend.Birthday).Year;

                        if (nextBirthday.Month - today.Month <= monthRange)
                        {
                            // Birthday is within 3 months
                            EventListItem newBirthday = new EventListItem();

                            newBirthday.OwnerID = friend.OwnerID;
                            newBirthday.OwnerName = friend.OwnerName;
                            newBirthday.WishListID = -1;
                            newBirthday.EventName = string.Format($"{friend.OwnerName}'s Birthday (Age {nextAge})");
                            newBirthday.EventDate = nextBirthday;
                            newBirthday.DaysRemaining = (int)((TimeSpan)(nextBirthday - today)).TotalDays;

                            events.Add(newBirthday);
                        }
                    }


                    foreach (var wishList in friend.WishLists)
                    {
                        if(wishList.DueDate <= DateTime.Today.AddMonths(monthRange))
                        {
                            EventListItem newEvent = new EventListItem();

                            newEvent.OwnerID = friend.OwnerID;
                            newEvent.OwnerName = friend.OwnerName;
                            newEvent.WishListID = wishList.WishListID;
                            newEvent.EventName = wishList.Name;
                            newEvent.EventDate = (DateTime)wishList.DueDate;
                            newEvent.DaysRemaining = (int)((TimeSpan)((DateTime)wishList.DueDate - today)).TotalDays;

                            events.Add(newEvent);
                        }                        
                    }
                }

                return events.OrderBy(e => e.EventDate);
            }
        }
    }
}
