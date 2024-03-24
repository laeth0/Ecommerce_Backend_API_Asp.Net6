using Ecommerce.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Core.IGenericRepository
{
    public interface IProductRepository:IGenericRepository<Product>
    {
        public Task<IReadOnlyList<Product>> GetAllAsync();
        public Task<Product> GetByIdAsync(int id);

    }
}
