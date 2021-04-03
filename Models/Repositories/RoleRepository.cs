using DigitalPayments.Data;
using DigitalPayments.Models.Repositories;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DigitalPayments.Models.Repositories
{

    public class RoleRepository : IDigitalPaymentsRepository<IdentityRole, string>
    {
        ApplicationDbContext db;

        public RoleRepository(ApplicationDbContext _db)
        {
            db = _db;
        }


        public void Add(IdentityRole role)
        {
            db.Roles.Add(role);
            db.SaveChanges();
        }

        public void Delete(string id)
        {
            var role = Find(id);
            db.Roles.Remove(role);
            db.SaveChanges();
        }

        public IdentityRole Find(string id)
        {
            var role = db.Roles.SingleOrDefault(q => q.Id == id);
            return role;
        }

        public IList<IdentityRole> List()
        {
            return db.Roles.ToList();
        }

        public void Update(IdentityRole newRole)
        {
            db.Update(newRole);
            db.SaveChanges();
        }
    }
}
