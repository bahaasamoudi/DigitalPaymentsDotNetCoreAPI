using DigitalPayments.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DigitalPayments.Models.Repositories
{

    public class TransferRepository : IDigitalPaymentsRepository<Transfer, int>
    {
        ApplicationDbContext db;

        public TransferRepository(ApplicationDbContext _db)
        {
            db = _db;
        }


        public void Add(Transfer transfer)
        {
            db.Transfers.Add(transfer);
            db.SaveChanges();
        }


        public void Delete(int id)
        {
            var transfer = Find(id);
            db.Transfers.Remove(transfer);
            db.SaveChanges();
        }

        public Transfer Find(int id)
        {
            var transfer = db.Transfers.SingleOrDefault(q => q.Id == id);
            return transfer;
        }

        public IList<Transfer> List()
        {
            return db.Transfers.ToList();
        }

        public void Update(Transfer transfer)
        {
            db.Update(transfer);
            db.SaveChanges();
        }

    }
}
