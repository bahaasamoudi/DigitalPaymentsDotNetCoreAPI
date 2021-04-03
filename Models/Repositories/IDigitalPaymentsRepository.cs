using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DigitalPayments.Models.Repositories
{
    public interface IDigitalPaymentsRepository<TEntity, KeyType>
    {
        IList<TEntity> List();
        TEntity Find(KeyType id);
        void Add(TEntity entity);
        void Update(TEntity entity);
        void Delete(KeyType id);
    }
}
