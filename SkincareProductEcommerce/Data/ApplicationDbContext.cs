using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using SkincareProductEcommerce.Models;

namespace SkincareProductEcommerce.Data
{
    public class ApplicationDbContext: IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
                
        }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<ShoppingCart> ShoppingCarts { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "History", DisplayOrder = 1 },
                new Category { Id = 2, Name = "Sci-fi", DisplayOrder = 3 },
                new Category { Id = 3, Name = "Action", DisplayOrder = 2 }
                );
            modelBuilder.Entity<Product>().HasData(
                new Product
                {
                    Id = 1,
                    Name = "Hydrating Serum",
                    Description = "A lightweight serum that hydrates and plumps skin",
                    Price = 25,
                    Size = "30ml",
                    CategoryId = 1,
                    ImageUrl = ""
                },
                new Product
                {
                    Id = 2,
                    Name = "Acne Cleanser ",
                    Description = "Gentle cleanser for acne-prone skin",
                    Price = 15,
                    Size = "150ml",
                    CategoryId = 2,
                    ImageUrl = ""
                },
                new Product
                {
                    Id = 3,
                    Name = "Vitamin C Cream",
                    Description = "Brightening cream enriched with Vitamin C",
                    Price = 30,
                    Size = "50ml",
                    CategoryId = 1,
                    ImageUrl = ""
                },
                new Product
                {
                    Id = 4,
                    Name = "Retinol Serum",
                    Description = "Anti-aging serum with retinol",
                    Price = 35,
                    Size = "50ml",
                    CategoryId = 3,
                    ImageUrl = ""
                },
                new Product
                {
                    Id = 5,
                    Name = "SPF 50 Sunscreen",
                    Description = "Broad-spectrum sunscreen with SPF",
                    Price = 20,
                    Size = "100ml",
                    CategoryId = 2,
                    ImageUrl = ""
                }
                );
        }
    }
}
