using DigitalPayments.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DigitalPayments.Models.Repositories
{

    public class ShopRepository : IDigitalPaymentsRepository<Shop, string>
    {
        ApplicationDbContext db;

        public ShopRepository(ApplicationDbContext _db)
        {
            db = _db;
        }


        public void Add(Shop shop)
        {
            db.Shops.Add(shop);
            db.SaveChanges();
        }


        public void Delete(string id)
        {
            var shop = Find(id);
            db.Shops.Remove(shop);
            db.SaveChanges();
        }

        public Shop Find(string id)
        {
            var shop = db.Shops.SingleOrDefault(q => q.Id == id);
            return shop;
        }

        public IList<Shop> List()
        {
            return db.Shops.ToList();
        }

        public void Update(Shop shop)
        {
            db.Update(shop);
            db.SaveChanges();
        }

    }
}
