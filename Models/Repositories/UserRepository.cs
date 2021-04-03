using DigitalPayments.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DigitalPayments.Models.Repositories
{

    public class UserRepository : IDigitalPaymentsRepository<ApplicationUser, string>
    {
        ApplicationDbContext db;

        public UserRepository(ApplicationDbContext _db)
        {
            db = _db;
        }


        public void Add(ApplicationUser user)
        {
            db.Users.Add(user);
            db.SaveChanges();
        }

        public void Delete(string id)
        {
            var user = Find(id.ToString());
            db.Users.Remove(user);
            db.SaveChanges();
        }



        public ApplicationUser Find(string id)
        {
            var user = db.Users.SingleOrDefault(q => q.Id == id);
            return user;
        }

       

        public IList<ApplicationUser> List()
        {
            return db.Users.ToList();
        }

        public void Update(ApplicationUser newuser)
        {
            db.Update(newuser);
            db.SaveChanges();
        }
    }
}
