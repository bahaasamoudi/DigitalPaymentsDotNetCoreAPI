using DigitalPayments.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DigitalPayments.Models.Repositories
{

    public class MessageRepository : IDigitalPaymentsRepository<Message, int>
    {
        ApplicationDbContext db;

        public MessageRepository(ApplicationDbContext _db)
        {
            db = _db;
        }


        public void Add(Message message)
        {
            db.Messages.Add(message);
            db.SaveChanges();
        }


        public void Delete(int id)
        {
            var message = Find(id);
            db.Messages.Remove(message);
            db.SaveChanges();
        }

        public Message Find(int id)
        {
            var message = db.Messages.SingleOrDefault(q => q.Id == id);
            return message;
        }

        public IList<Message> List()
        {
            return db.Messages.ToList();
        }

        public void Update(Message message)
        {
            db.Update(message);
            db.SaveChanges();
        }

    }
}
