using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDS.Domain.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        IQueryable<T> GetAll();
        Task<T> GetByIdAsync(object id);

        T GetById(object id);
        Task<T> Insert(T obj);
        Task Update(T obj);
        void Delete(object id);
        void Save();
    }
}
