using Ecommerce.Core.IGenericRepository;
using Ecommerce.Repository.Data;

namespace Ecommerce.Repository.GenericRepository
{
    public class UnitOfWork : IUnitOfWork
    {
        public IProductRepository productRepository { get; set; }
        public ICategoryRepository categoryRepository { get; set; }
        public ICartRepository cartRepository { get; set; }

        private readonly EcommerceContext dbContext;

        public UnitOfWork(EcommerceContext _dbContext)
        {
            dbContext = _dbContext;
            productRepository = new ProductRepository(dbContext);
            categoryRepository = new CategoryRepository(dbContext);
            cartRepository = new CartRepository(dbContext);
        }

        public async Task<int> SaveChangesAsync() => await dbContext.SaveChangesAsync();

    }
}
