using Ecommerce.Core.Models;


namespace Ecommerce.Core.IGenericRepository
{
    public interface IProductRepository:IGenericRepository<Product>
    {
        public Task<IReadOnlyList<Product>> GetAllAsync();
        public Task<Product> GetByIdAsync(int id);
        public Task<IReadOnlyList<Product>> GetProductsByCategoryAsync(int categoryId);
    }
}
