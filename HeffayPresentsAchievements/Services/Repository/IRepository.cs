using HeffayPresentsAchievements.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HeffayPresentsAchievements.Services.Repository
{
    // https://www.c-sharpcorner.com/article/generic-repository-pattern-in-asp-net-core/
    public interface IRepository<T> where T : BaseEntity
    {
        public Task<IEnumerable<T?>> GetAll();
        public Task<IEnumerable<T?>> GetAllForId(Guid id);
        public Task<T?> Get(Guid Id);
        public Task<int> Add(T entity);
        public Task<int> AddRange(IEnumerable<T> entities);
        public Task Update(T entity);
        public Task<int> Remove(Guid id);
        public Task<int> RemoveRange(IEnumerable<T> entities);
    }
}
