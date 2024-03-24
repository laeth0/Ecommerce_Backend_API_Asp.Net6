using Ecommerce.Core.IGenericRepository;
using Ecommerce.Core.Models;
using Ecommerce.Repository.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Repository.GenericRepository
{
    public class CategoryRepository:GenericRepository<Category>,ICategoryRepository
    {
        private readonly EcommerceContext dbContext;

        public CategoryRepository(EcommerceContext dbContext) : base(dbContext)
        {
            this.dbContext = dbContext;
        }
    }
    
}
