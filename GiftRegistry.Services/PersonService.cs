using GiftRegistry.Data;
using GiftRegistry.Models;
using System;
using System.Collections.Generic;
using System.Linq;
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
                    Birthdate = model.Birthdate
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
                        .Where(e => e.PersonGUID == _userId)
                        .Select(
                            e =>
                                new PersonListItem
                                {
                                    PersonID = e.PersonID,
                                    FullName = e.FirstName + " " + e.LastName,
                                    Birthdate = e.Birthdate
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
                        Birthdate = entity.Birthdate
                    };
            }
        }

        public PersonDetail GetPersonByGUID()
        {
            using (var ctx = new ApplicationDbContext())
            {
                // If this user doesn't exist yet (new registration), create a person for them
                if(!ctx.People.Any(e => e.PersonGUID == _userId))
                {
                    PersonCreate newPerson = new PersonCreate();
                    newPerson.FirstName = "First Name";
                    newPerson.LastName = "Last Name";
                    newPerson.Birthdate = DateTime.Now;

                    if (!CreatePerson(newPerson))
                        return null;
                }

                var entity =
                    ctx
                        .People
                        .Single(e => e.PersonGUID == _userId);

                return
                    new PersonDetail
                    {
                        PersonID = entity.PersonID,                        
                        FirstName = entity.FirstName,
                        LastName = entity.LastName,
                        Birthdate = entity.Birthdate
                    };
            }
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

                return ctx.SaveChanges() == 1;
            }
        }

        public bool DeletePerson(int id)
        {
            using (var ctx = new ApplicationDbContext())
            {
                var entity =
                    ctx
                        .People
                        .Single(e => e.PersonID == id && e.PersonGUID == _userId);

                ctx.People.Remove(entity);

                return ctx.SaveChanges() == 1;
            }
        }
    }
}
