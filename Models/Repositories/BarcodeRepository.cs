using DigitalPayments.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DigitalPayments.Models.Repositories
{

    public class BarcodeRepository : IDigitalPaymentsRepository<Barcode, int>
    {
        ApplicationDbContext db;

        public BarcodeRepository(ApplicationDbContext _db)
        {
            db = _db;
        }


        public void Add(Barcode barcode)
        {
            db.Barcodes.Add(barcode);
            db.SaveChanges();
        }


        public void Delete(int id)
        {
            var barcode = Find(id);
            db.Barcodes.Remove(barcode);
            db.SaveChanges();
        }

        public Barcode Find(int id)
        {
            var barcode = db.Barcodes.SingleOrDefault(q => q.Id == id);
            return barcode;
        }

        public IList<Barcode> List()
        {
            return db.Barcodes.ToList();
        }

        public void Update(Barcode barcode)
        {
            db.Update(barcode);
            db.SaveChanges();
        }

    }
}
