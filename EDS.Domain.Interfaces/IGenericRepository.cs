using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EDS.Domain.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAll();
        Task<T> GetById(object id);
        Task<T> Insert(T obj);
        Task Update(T obj);
        void Delete(object id);
        void Save();
    }
}
