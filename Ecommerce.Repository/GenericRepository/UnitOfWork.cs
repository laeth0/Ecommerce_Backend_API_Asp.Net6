using Ecommerce.Core.IGenericRepository;
using Ecommerce.Repository.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Repository.GenericRepository
{
    public class UnitOfWork : IUnitOfWork
    {
        public IProductRepository productRepository { get; set; }
        public ICategoryRepository categoryRepository { get; set; }

        private readonly EcommerceContext dbContext;

        public UnitOfWork(EcommerceContext _dbContext)
        {
            dbContext = _dbContext;
            productRepository = new ProductRepository(dbContext);
            categoryRepository = new CategoryRepository(dbContext);
        }

        public async Task<int> SaveChangesAsync() => await dbContext.SaveChangesAsync();

    }
}
