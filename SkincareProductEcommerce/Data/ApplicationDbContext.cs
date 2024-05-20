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
        public DbSet<OrderHeader> OrderHeaders { get; set; }
        public DbSet<OrderDetails> OrderDetails { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Cleansers & Lotions", DisplayOrder = 1 },
                new Category { Id = 2, Name = "Face Masks", DisplayOrder = 3 },
                new Category { Id = 3, Name = "Face Serums", DisplayOrder = 2 },
                new Category { Id = 4, Name = "Creams and Emulsions", DisplayOrder = 2 }
                );
            modelBuilder.Entity<Product>().HasData(
                new Product
                {
                    Id = 1,
                    Name = "Purifying Light Foam",
                    Description = "Lorem ipsum dolor sit amet. Non praesentium quia ut rerum nihil ab esse quisquam ut nemo Quis eum tempore minus ad mollitia delectus ut illum commodi. Vel galisum rerum est harum debitis est odio dicta et fugiat dicta ut quia molestias hic maiores nemo et molestiae ullam. Ab sint expedita et voluptatem fugiat quo commodi harum ea iusto molestiae. ",
                    Price = 25,
                    Size = "30ml",
                    CategoryId = 1,
                    ImageUrl = ""
                },
                new Product
                {
                    Id = 2,
                    Name = "Hydra-Mask",
                    Description = "Lorem ipsum dolor sit amet. Non praesentium quia ut rerum nihil ab esse quisquam ut nemo Quis eum tempore minus ad mollitia delectus ut illum commodi. Vel galisum rerum est harum debitis est odio dicta et fugiat dicta ut quia molestias hic maiores nemo et molestiae ullam. Ab sint expedita et voluptatem fugiat quo commodi harum ea iusto molestiae. ",
                    Price = 15,
                    Size = "150ml",
                    CategoryId = 2,
                    ImageUrl = ""
                },
                new Product
                {
                    Id = 3,
                    Name = "Anti-Age Defence Mask",
                    Description = "Lorem ipsum dolor sit amet. Non praesentium quia ut rerum nihil ab esse quisquam ut nemo Quis eum tempore minus ad mollitia delectus ut illum commodi. Vel galisum rerum est harum debitis est odio dicta et fugiat dicta ut quia molestias hic maiores nemo et molestiae ullam. Ab sint expedita et voluptatem fugiat quo commodi harum ea iusto molestiae. ",
                    Price = 30,
                    Size = "50ml",
                    CategoryId = 2,
                    ImageUrl = ""
                },
                new Product
                {
                    Id = 4,
                    Name = "Micellar Water Face & Eyes",
                    Description = "Lorem ipsum dolor sit amet. Non praesentium quia ut rerum nihil ab esse quisquam ut nemo Quis eum tempore minus ad mollitia delectus ut illum commodi. Vel galisum rerum est harum debitis est odio dicta et fugiat dicta ut quia molestias hic maiores nemo et molestiae ullam. Ab sint expedita et voluptatem fugiat quo commodi harum ea iusto molestiae. ",
                    Price = 35,
                    Size = "50ml",
                    CategoryId = 1,
                    ImageUrl = ""
                },
                new Product
                {
                    Id = 5,
                    Name = "Anti-Age Booster Serum",
                    Description = "Lorem ipsum dolor sit amet. Non praesentium quia ut rerum nihil ab esse quisquam ut nemo Quis eum tempore minus ad mollitia delectus ut illum commodi. Vel galisum rerum est harum debitis est odio dicta et fugiat dicta ut quia molestias hic maiores nemo et molestiae ullam. Ab sint expedita et voluptatem fugiat quo commodi harum ea iusto molestiae. ",
                    Price = 20,
                    Size = "100ml",
                    CategoryId = 3,
                    ImageUrl = ""
                }
                );
        }
    }
}
