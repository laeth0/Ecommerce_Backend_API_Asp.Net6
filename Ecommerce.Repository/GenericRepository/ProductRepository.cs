﻿using Ecommerce.Core.IGenericRepository;
using Ecommerce.Core.Models;
using Ecommerce.Repository.Data;
using Microsoft.EntityFrameworkCore;


namespace Ecommerce.Repository.GenericRepository
{
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        private readonly EcommerceContext dbContext;

        public ProductRepository(EcommerceContext dbContext) : base(dbContext)
        {
            this.dbContext = dbContext;
        }

        public new async Task<IReadOnlyList<Product>> GetAllAsync()
        {
            return await dbContext.Products.Include(p => p.Category).AsNoTracking().ToListAsync();
        }

        public new async Task<Product> GetByIdAsync(int id)
        {
            return await dbContext.Products.Include(p => p.Category).FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IReadOnlyList<Product>> GetProductsByCategoryAsync(int categoryId)
        {
            return await dbContext.Products.Include(p => p.Category).Where(p => p.CategoryId == categoryId).ToListAsync();
        }
      
    }
}
