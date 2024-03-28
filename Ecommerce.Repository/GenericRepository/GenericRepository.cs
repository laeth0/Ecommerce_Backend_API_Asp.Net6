using Ecommerce.Core.IGenericRepository;
using Ecommerce.Core.Models;
using Ecommerce.Repository.Data;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Repository.GenericRepository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly EcommerceContext dbContext;

        public GenericRepository(EcommerceContext dbContext)
        {
            this.dbContext = dbContext;
        }


        // i am using IReadOnlyList instead of IEnumerable becouse the IReadOnlyList is faster than IEnumerable
        public async Task<IReadOnlyList<T>> GetAllAsync() => await dbContext.Set<T>().AsNoTracking().ToListAsync();

        public async Task<T?> GetByIdAsync(int id) => await dbContext.Set<T>().FindAsync(id);

        public async Task AddAsync(T entity) => await dbContext.Set<T>().AddAsync(entity);

        public async Task Delete(int id)
        {
            T? entity= await dbContext.Set<T>().FindAsync(id);
            dbContext.Set<T>().Remove(entity);
        }

        public void Update(T entity) => dbContext.Set<T>().Update(entity);

    }
}
