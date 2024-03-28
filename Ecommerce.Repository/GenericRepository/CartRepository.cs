using Ecommerce.Core.IGenericRepository;
using Ecommerce.Core.Models;
using Ecommerce.Repository.Data;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Repository.GenericRepository
{
    public class CartRepository : GenericRepository<Cart>, ICartRepository
    {
        private readonly EcommerceContext dbContext;

        public CartRepository(EcommerceContext dbContext) : base(dbContext)
        {
            this.dbContext = dbContext;
        }


        public async Task<IReadOnlyList<Cart>?> GetCartItemsByUserIdAsync(string userId)
        {
            return await dbContext.CartItems.Where(x => x.UserId == userId).ToListAsync();
        }


        public async Task<Cart?> GetCartItemByUserIdAndProductIdAsync(string userId, int productId)
        {
            return await dbContext.CartItems.FirstOrDefaultAsync(C => C.UserId == userId && C.ProductId == productId);
        }


        public async Task ClearCart(string userId)
        {
            var CartItems = await dbContext.CartItems.Where(x => x.UserId == userId).ToListAsync();
            foreach (Cart item in CartItems)
                dbContext.CartItems.Remove(item);
        }

    }
}
