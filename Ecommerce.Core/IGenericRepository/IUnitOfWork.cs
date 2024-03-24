using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Core.IGenericRepository
{
    public interface IUnitOfWork
    {
        public IProductRepository productRepository { get; set; }

        public ICategoryRepository categoryRepository { get; set; }

        public Task<int> SaveChangesAsync();
    }
}
