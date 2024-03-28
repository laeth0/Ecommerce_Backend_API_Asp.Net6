using Ecommerce.Core.Models;

namespace Ecommerce.Core.IGenericRepository
{
    public interface ICartRepository : IGenericRepository<Cart>
    {
        public Task<IReadOnlyList<Cart>?> GetCartItemsByUserIdAsync(string userId);

        public Task<Cart?> GetCartItemByUserIdAndProductIdAsync(string userId, int productId);

        public Task ClearCart(string userId);

    }
}
