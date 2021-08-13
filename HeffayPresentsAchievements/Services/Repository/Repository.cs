using HeffayPresentsAchievements.Data;
using HeffayPresentsAchievements.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HeffayPresentsAchievements.Services.Repository
{
    public class Repository<T> : IRepository<T> where T: BaseEntity
    {
        private readonly DataContext _context;
        private readonly DbSet<T> entities;

        public Repository(DataContext context)
        {
            _context = context;
            entities = context.Set<T>();
        }

        public async Task<IEnumerable<T>> GetAll()
        {
            return entities.AsEnumerable();
        }

        public async Task<T> Get(Guid Id)
        {
            T? result = entities.SingleOrDefault(a => a.Id == Id);
            if (result == null)
            {
                throw new ApplicationException($"Unable to find record with Id of {Id}");
            }
            return result;
        }

        public async Task<int> Add(T record)
        {
            if (record == null)
            {
                throw new ArgumentNullException(nameof(record));
            }
            entities.Add(record);
            var result = await _context.SaveChangesAsync();
            return result;
        }

        public async Task<int> AddRange(IEnumerable<T> records)
        {
            if (records == null)
            {
                throw new ArgumentNullException(nameof(records));
            }
            entities.AddRange(records);
            var result = await _context.SaveChangesAsync();
            return result;
        }

        public async Task Update(T record)
        {
            _context.Update(record);
            await _context.SaveChangesAsync();
        }

        public async Task<int> Remove(Guid id)
        {
            var record = entities.FirstOrDefault(a => a.Id.Equals(id));
            if (record != null)
            {
                entities.Remove(record);
                return await _context.SaveChangesAsync();
            }
           
            return 0;
        }

        public async Task<int> RemoveRange(IEnumerable<T> records)
        {
            if (records == null)
            {
                throw new ArgumentNullException(nameof(records));
            }
            foreach (var record in records)
            {
                entities.Remove(record);
            }
            var result = await _context.SaveChangesAsync();
            return result;
        }
    }
}
