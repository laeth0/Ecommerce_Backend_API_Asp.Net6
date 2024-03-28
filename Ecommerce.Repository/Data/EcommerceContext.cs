using Ecommerce.Core.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Repository.Data
{
    public class EcommerceContext : IdentityDbContext<User>
    {
        public EcommerceContext(DbContextOptions<EcommerceContext> options): base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<Category>()
            //    .HasMany(c => c.Products)
            //    .WithOne(p => p.Category)
            //    .HasForeignKey(p => p.CategoryId)
            //    .OnDelete(DeleteBehavior.SetNull);


            //modelBuilder.Entity<Product>()
            //    .HasMany(C => C.CartItems)
            //    .WithOne(p => p.Product)
            //    .HasForeignKey(C=>C.ProductId)
            //    .OnDelete(DeleteBehavior.SetNull);

            

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Cart> CartItems { get; set; }

    }
}
