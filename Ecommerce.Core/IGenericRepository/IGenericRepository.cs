
namespace Ecommerce.Core.IGenericRepository
{
    public interface IGenericRepository<T> where T : class
    {
        Task<IReadOnlyList<T>> GetAllAsync();
        Task<T?> GetByIdAsync(int id);

        Task AddAsync(T entity);
        void Update(T entity);
        Task Delete(int id);

    }

}
