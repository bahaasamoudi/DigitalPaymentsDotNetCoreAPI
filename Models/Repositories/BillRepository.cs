using DigitalPayments.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DigitalPayments.Models.Repositories
{

    public class BillRepository : IDigitalPaymentsRepository<Bill, int>
    {
        ApplicationDbContext db;

        public BillRepository(ApplicationDbContext _db)
        {
            db = _db;
        }


        public void Add(Bill bill)
        {
            db.Bills.Add(bill);
            db.SaveChanges();
        }


        public void Delete(int id)
        {
            var bill = Find(id);
            db.Bills.Remove(bill);
            db.SaveChanges();
        }

        public Bill Find(int id)
        {
            var bill = db.Bills.SingleOrDefault(q => q.Id == id);
            return bill;
        }

        public IList<Bill> List()
        {
            return db.Bills.ToList();
        }

        public void Update(Bill bill)
        {
            db.Update(bill);
            db.SaveChanges();
        }

    }
}
