

namespace Ecommerce.Core.IGenericRepository
{
    public interface IUnitOfWork
    {
        public IProductRepository productRepository { get; set; }

        public ICategoryRepository categoryRepository { get; set; }
        public ICartRepository cartRepository { get; set; }

        public Task<int> SaveChangesAsync();
    }
}
