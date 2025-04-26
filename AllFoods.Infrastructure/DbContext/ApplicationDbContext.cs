using AllFoods.Core.Domain.Entities;
using AllFoods.Core.Domain.IdentityEntities;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace AllFoods.Infrastructure.DbContext
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

        public DbSet<ApplicationUser> ApplicationUsers => Set<ApplicationUser>();
        public virtual DbSet<Category> Categories => Set<Category>();
        public virtual DbSet<Product> Products => Set<Product>();
        public virtual DbSet<Cart> Carts => Set<Cart>();
        public virtual DbSet<CartItem> CartItems => Set<CartItem>();
        public virtual DbSet<Order> Orders => Set<Order>();
        public virtual DbSet<OrderItem> OrderItems => Set<OrderItem>();


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Cart>()
                .HasMany(c => c.Items)
                .WithOne(ci => ci.Cart)
                .HasForeignKey(ci => ci.CartID);

            modelBuilder.Entity<CartItem>()
               .HasKey(ci => new { ci.CartID, ci.ProductID });

            modelBuilder.Entity<Category>().ToTable("Categories");
            modelBuilder.Entity<Product>().ToTable("Products");

            string categoryStr = File.ReadAllText("categories.json");
            var categories = JsonSerializer.Deserialize <List<Category>>(categoryStr);
            if(categories is null || categories.Count == 0)
            {
                throw new ArgumentNullException(nameof(categories));
            }
            foreach (var item in categories)
            {
                modelBuilder.Entity<Category>().HasData(item);
            }


            // Product

            string productStr = File.ReadAllText("products.json");
            var products = JsonSerializer.Deserialize<List<Product>>(productStr);
            if (products is null || products.Count == 0)
            {
                throw new ArgumentNullException(nameof(products));
            }
            foreach (var item in products)
            {
                modelBuilder.Entity<Product>().HasData(item);
            }
        }
        


    }
}
