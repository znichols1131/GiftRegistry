using GiftRegistry.Data;
using GiftRegistry.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GiftRegistry.Services
{
    public class PersonService
    {
        private readonly Guid _userId;

        public PersonService(Guid userId)
        {
            _userId = userId;
        }

        public bool CreatePerson(PersonCreate model)
        {
            var entity =
                new Person()
                {
                    PersonGUID = _userId,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Birthdate = model.Birthdate,
                    ProfilePicture = CreateDefaultImageModel().ImageData
                };

            using (var ctx = new ApplicationDbContext())
            {
                ctx.People.Add(entity);
                return ctx.SaveChanges() == 1;
            }
        }

        public IEnumerable<PersonListItem> GetPeople()
        {
            using (var ctx = new ApplicationDbContext())
            {
                var query =
                    ctx
                        .People
                        .Where(e => e.PersonGUID != _userId)
                        .OrderBy(f => f.LastName)
                        .ThenBy(f => f.FirstName)
                        .Select(
                            e =>
                                new PersonListItem
                                {
                                    PersonID = e.PersonID,
                                    FullName = e.FirstName + " " + e.LastName,
                                    Birthdate = e.Birthdate,
                                    ProfilePicture = e.ProfilePicture
                                }
                        );

                return query.ToArray();
            }
        }

        public IEnumerable<PersonListItem_Stranger> GetStrangers()
        {
            using (var ctx = new ApplicationDbContext())
            {

                // If the user has a Person associated with them, filter out any Friends from the list of Strangers
                if (ctx.People.Any(p => p.PersonGUID == _userId) && ctx.Friends.Any(f => f.OwnerGUID == _userId))
                {
                    var strangers = ctx.People  .Where(e => e.PersonGUID != _userId && 
                                                            !ctx.Friends.Any(f => f.OwnerGUID == _userId && f.PersonID == e.PersonID))
                                                .OrderBy(f => f.LastName)
                                                .ThenBy(f => f.FirstName)
                                                .Select(e =>
                                                            new PersonListItem_Stranger
                                                            {
                                                                PersonID = e.PersonID,
                                                                FullName = e.FirstName + " " + e.LastName,
                                                                ProfilePicture = e.ProfilePicture
                                                            }
                                                );

                    return strangers.ToArray();
                }

                var query = ctx.People  .Where(e => e.PersonGUID != _userId)
                                        .OrderBy(f => f.LastName)
                                        .ThenBy(f => f.FirstName)
                                        .Select(e =>
                                                new PersonListItem_Stranger
                                                {
                                                    PersonID = e.PersonID,
                                                    FullName = e.FirstName + " " + e.LastName,
                                                    ProfilePicture = e.ProfilePicture
                                                }
                                        );

                return query.ToArray();
            }
        }

        public IEnumerable<PersonListItem_Stranger> GetStrangersForSearchString(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return null;

            input = input.Trim();

            using (var ctx = new ApplicationDbContext())
            {

                // If the user has a Person associated with them, filter out any Friends from the list of Strangers
                if (ctx.People.Any(p => p.PersonGUID == _userId) && ctx.Friends.Any(f => f.OwnerGUID == _userId))
                {
                    var strangers = ctx.People.Where(e => e.PersonGUID != _userId &&
                                                          !ctx.Friends.Any(f => f.OwnerGUID == _userId && f.PersonID == e.PersonID) &&
                                                          (e.FirstName.ToLower().Contains(input.ToLower()) || e.LastName.ToLower().Contains(input.ToLower())))
                                                .OrderBy(f => f.LastName)
                                                .ThenBy(f => f.FirstName)
                                                .Select(e =>
                                                            new PersonListItem_Stranger
                                                            {
                                                                PersonID = e.PersonID,
                                                                FullName = e.FirstName + " " + e.LastName,
                                                                ProfilePicture = e.ProfilePicture
                                                            }
                                                );

                    return strangers.ToArray();
                }

                var query = ctx.People.Where(e => e.PersonGUID != _userId &&
                                            (e.FirstName.ToLower().Contains(input.ToLower()) || e.LastName.ToLower().Contains(input.ToLower())))
                                        .OrderBy(f => f.LastName)
                                        .ThenBy(f => f.FirstName)
                                        .Select(e =>
                                                new PersonListItem_Stranger
                                                {
                                                    PersonID = e.PersonID,
                                                    FullName = e.FirstName + " " + e.LastName,
                                                    ProfilePicture = e.ProfilePicture
                                                }
                                        );

                return query.ToArray();
            }
        }

        public PersonDetail GetPersonById(int id)
        {
            using (var ctx = new ApplicationDbContext())
            {
                var entity =
                    ctx
                        .People
                        .Single(e => e.PersonID == id && e.PersonGUID == _userId);

                return
                    new PersonDetail
                    {
                        PersonID = entity.PersonID,
                        FirstName = entity.FirstName,
                        LastName = entity.LastName,
                        Birthdate = entity.Birthdate,
                        Image = (entity.ProfilePicture == null || entity.ProfilePicture.Length == 0) ? CreateDefaultImageModel() : CreateImageModelForBytes(entity.ProfilePicture)
                    };
            }
        }

        public PersonDetail GetPersonByGUID(Guid guid)
        {
            using (var ctx = new ApplicationDbContext())
            {
                if(!ctx.People.Any(e => e.PersonGUID == guid))
                {
                    return null;
                }

                var entity =
                    ctx
                        .People
                        .Single(e => e.PersonGUID == guid);

                return
                    new PersonDetail
                    {
                        PersonID = entity.PersonID,
                        FirstName = entity.FirstName,
                        LastName = entity.LastName,
                        Birthdate = entity.Birthdate,
                        Image = (entity.ProfilePicture == null || entity.ProfilePicture.Length == 0) ? CreateDefaultImageModel() : CreateImageModelForBytes(entity.ProfilePicture)
                    };
            }
        }

        public PersonDetail GetCurrentPerson()
        {
            return GetPersonByGUID(_userId);
        }

        public bool UpdatePerson(PersonEdit model)
        {
            using (var ctx = new ApplicationDbContext())
            {
                var entity =
                    ctx
                        .People
                        .Single(e => e.PersonID == model.PersonID && e.PersonGUID == _userId);

                entity.FirstName = model.FirstName;
                entity.LastName = model.LastName;
                entity.Birthdate = model.Birthdate;
                entity.ProfilePicture = model.Image.ImageData;

                return ctx.SaveChanges() == 1;
            }
        }

        public bool DeletePerson(int id)
        {
            using (var ctx = new ApplicationDbContext())
            {
                if (ctx.People.Any(e => e.PersonID == id))
                {
                    var entity =
                    ctx
                        .People
                        .Include("WishLists")
                        .Single(e => e.PersonID == id);

                    // Don't rely on cascade deleting. We want to send notification to buyers whose transactions will be deleted
                    var wishListService = CreateWishListService();
                    foreach (var gift in entity.WishLists)
                    {
                        wishListService.DeleteWishList(gift.WishListID);
                    }

                    ctx.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                    ctx.People.Remove(entity);

                    return ctx.SaveChanges() > 0;
                }                
            }

            return false;
        }

        private ImageService CreateImageService()
        {
            var service = new ImageService(_userId);
            return service;
        }

        private WishListService CreateWishListService()
        {
            var service = new WishListService(_userId);
            return service;
        }

        private ImageModel CreateImageModelForBytes(byte[] input)
        {
            var service = CreateImageService();

            service.DeleteImagesForUser();

            var model = new ImageModel();
            model.ImageData = input;
            model.OwnerGUID = _userId;

            if (!service.CreateImage(model))
                return null;

            return service.GetLatestImageForUser();
        }

        private ImageModel CreateDefaultImageModel()
        {
            var service = CreateImageService();

            service.DeleteImagesForUser();

            if (!service.CreateDefaultImage(true))
                return null;

            var image = service.GetLatestImageForUser();

            return image;
        }
    }
}
