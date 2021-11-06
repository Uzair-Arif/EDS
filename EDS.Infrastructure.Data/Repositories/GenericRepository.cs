using EDS.Domain.Interfaces;
using EM.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDS.Infrastructure.Data.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        public AppDbContext _context = null;
        public DbSet<T> table = null;
        public GenericRepository()
        {
            this._context = new AppDbContext();
            table = _context.Set<T>();
        }
        public GenericRepository(AppDbContext _context)
        {
            this._context = _context;
            table = _context.Set<T>();
        }
        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await table.ToListAsync();
        }


        public IQueryable<T> GetAll()
        {
            return _context.Set<T>();
        }

        public async Task<T> GetByIdAsync(object id)
        {
            return await table.FindAsync(id);
        }

        public T GetById(object id)
        {
            return _context.Set<T>().Find(id);
        }
        public async Task<T> Insert(T obj)
        {
             await table.AddAsync(obj);
             await _context.SaveChangesAsync();
            return obj;
        }
        public Task Update(T obj)
        {
            _context.Entry(obj).State = EntityState.Modified;
            return _context.SaveChangesAsync();
            
        }
        public void Delete(object id)
        {
            T existing = table.Find(id);
            table.Remove(existing);
        }
        public void Save()
        {
            _context.SaveChanges();
        }
    }
}
