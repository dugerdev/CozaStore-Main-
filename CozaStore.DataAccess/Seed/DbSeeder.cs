using CozaStore.DataAccess.Data;
using CozaStore.Entities.Entities;
using CozaStore.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CozaStore.DataAccess.Seed;

public static class DbSeeder
{
    public static async Task SeedAsync(CozaStoreDbContext context, UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
    {
        // Ensure database is created
        await context.Database.MigrateAsync();

        // Seed Roles
        await SeedRolesAsync(roleManager);

        // Seed Users
        await SeedUsersAsync(userManager);

        // Seed Categories
        await SeedCategoriesAsync(context);

        // Seed Products
        await SeedProductsAsync(context);

        // Seed Blog Posts
        await SeedBlogPostsAsync(context, userManager);

        await context.SaveChangesAsync();
    }

    private static async Task SeedRolesAsync(RoleManager<ApplicationRole> roleManager)
    {
        var roles = new Dictionary<string, string>
        {
            { "Admin", "Administrator role with full access" },
            { "User", "Regular user role with limited access" }
        };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role.Key))
            {
                await roleManager.CreateAsync(new ApplicationRole 
                { 
                    Name = role.Key,
                    Description = role.Value
                });
            }
        }
    }

    private static async Task SeedUsersAsync(UserManager<ApplicationUser> userManager)
    {
        // Admin User
        var adminEmail = "admin@cozastore.com";
        if (await userManager.FindByEmailAsync(adminEmail) == null)
        {
            var adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true,
                FirstName = "Admin",
                LastName = "User"
            };

            var result = await userManager.CreateAsync(adminUser, "Admin123!");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }

        // Regular User
        var userEmail = "user@cozastore.com";
        if (await userManager.FindByEmailAsync(userEmail) == null)
        {
            var regularUser = new ApplicationUser
            {
                UserName = userEmail,
                Email = userEmail,
                EmailConfirmed = true,
                FirstName = "John",
                LastName = "Doe"
            };

            var result = await userManager.CreateAsync(regularUser, "User123!");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(regularUser, "User");
            }
        }
    }

    private static async Task SeedCategoriesAsync(CozaStoreDbContext context)
    {
        if (await context.Categories.AnyAsync())
            return;

        var categories = new List<Category>
        {
            new Category
            {
                Id = Guid.NewGuid(),
                Name = "Women",
                Description = "Women's fashion and accessories",
                IsActive = true,
                CreatedDate = DateTime.UtcNow
            },
            new Category
            {
                Id = Guid.NewGuid(),
                Name = "Men",
                Description = "Men's fashion and accessories",
                IsActive = true,
                CreatedDate = DateTime.UtcNow
            },
            new Category
            {
                Id = Guid.NewGuid(),
                Name = "Bag",
                Description = "Bags and backpacks",
                IsActive = true,
                CreatedDate = DateTime.UtcNow
            },
            new Category
            {
                Id = Guid.NewGuid(),
                Name = "Shoes",
                Description = "Footwear collection",
                IsActive = true,
                CreatedDate = DateTime.UtcNow
            },
            new Category
            {
                Id = Guid.NewGuid(),
                Name = "Watches",
                Description = "Watches and timepieces",
                IsActive = true,
                CreatedDate = DateTime.UtcNow
            }
        };

        await context.Categories.AddRangeAsync(categories);
        await context.SaveChangesAsync();
    }

    private static async Task SeedProductsAsync(CozaStoreDbContext context)
    {
        if (await context.Products.AnyAsync())
            return;

        var categories = await context.Categories.ToListAsync();
        var womenCategory = categories.First(c => c.Name == "Women");
        var menCategory = categories.First(c => c.Name == "Men");
        var bagCategory = categories.First(c => c.Name == "Bag");
        var shoesCategory = categories.First(c => c.Name == "Shoes");
        var watchesCategory = categories.First(c => c.Name == "Watches");

        var products = new List<Product>
        {
            // Women Products
            new Product
            {
                Id = Guid.NewGuid(),
                Name = "Esprit Ruffle Shirt",
                Description = "Stylish ruffle shirt perfect for any occasion",
                Price = 16.64m,
                StockQuantity = 50,
                ImageUrl = "/images/product-01.jpg",
                CategoryId = womenCategory.Id,
                IsActive = true,
                CreatedDate = DateTime.UtcNow
            },
            new Product
            {
                Id = Guid.NewGuid(),
                Name = "Herschel supply",
                Description = "Quality supply bag from Herschel",
                Price = 35.31m,
                StockQuantity = 30,
                ImageUrl = "/images/product-02.jpg",
                CategoryId = womenCategory.Id,
                IsActive = true,
                CreatedDate = DateTime.UtcNow
            },
            new Product
            {
                Id = Guid.NewGuid(),
                Name = "Classic Trench Coat",
                Description = "Elegant trench coat for stylish women",
                Price = 75.00m,
                StockQuantity = 20,
                ImageUrl = "/images/product-04.jpg",
                CategoryId = womenCategory.Id,
                IsActive = true,
                CreatedDate = DateTime.UtcNow
            },
            new Product
            {
                Id = Guid.NewGuid(),
                Name = "Front Pocket Jumper",
                Description = "Comfortable jumper with front pocket",
                Price = 34.75m,
                StockQuantity = 45,
                ImageUrl = "/images/product-05.jpg",
                CategoryId = womenCategory.Id,
                IsActive = true,
                CreatedDate = DateTime.UtcNow
            },
            new Product
            {
                Id = Guid.NewGuid(),
                Name = "Shirt in Stretch Cotton",
                Description = "Comfortable stretch cotton shirt",
                Price = 52.66m,
                StockQuantity = 35,
                ImageUrl = "/images/product-07.jpg",
                CategoryId = womenCategory.Id,
                IsActive = true,
                CreatedDate = DateTime.UtcNow
            },
            new Product
            {
                Id = Guid.NewGuid(),
                Name = "Pieces Metallic Printed",
                Description = "Trendy metallic printed pieces",
                Price = 18.96m,
                StockQuantity = 60,
                ImageUrl = "/images/product-08.jpg",
                CategoryId = womenCategory.Id,
                IsActive = true,
                CreatedDate = DateTime.UtcNow
            },
            new Product
            {
                Id = Guid.NewGuid(),
                Name = "Femme T-Shirt In Stripe",
                Description = "Stylish striped t-shirt for women",
                Price = 25.85m,
                StockQuantity = 55,
                ImageUrl = "/images/product-10.jpg",
                CategoryId = womenCategory.Id,
                IsActive = true,
                CreatedDate = DateTime.UtcNow
            },
            new Product
            {
                Id = Guid.NewGuid(),
                Name = "T-Shirt with Sleeve",
                Description = "Classic t-shirt with sleeve design",
                Price = 18.49m,
                StockQuantity = 70,
                ImageUrl = "/images/product-13.jpg",
                CategoryId = womenCategory.Id,
                IsActive = true,
                CreatedDate = DateTime.UtcNow
            },
            new Product
            {
                Id = Guid.NewGuid(),
                Name = "Pretty Little Thing",
                Description = "Pretty and stylish fashion piece",
                Price = 54.79m,
                StockQuantity = 25,
                ImageUrl = "/images/product-14.jpg",
                CategoryId = womenCategory.Id,
                IsActive = true,
                CreatedDate = DateTime.UtcNow
            },
            new Product
            {
                Id = Guid.NewGuid(),
                Name = "Square Neck Back",
                Description = "Elegant square neck back design",
                Price = 29.64m,
                StockQuantity = 40,
                ImageUrl = "/images/product-16.jpg",
                CategoryId = womenCategory.Id,
                IsActive = true,
                CreatedDate = DateTime.UtcNow
            },

            // Men Products
            new Product
            {
                Id = Guid.NewGuid(),
                Name = "Only Check Trouser",
                Description = "Stylish check trouser for men",
                Price = 25.50m,
                StockQuantity = 35,
                ImageUrl = "/images/product-03.jpg",
                CategoryId = menCategory.Id,
                IsActive = true,
                CreatedDate = DateTime.UtcNow
            },
            new Product
            {
                Id = Guid.NewGuid(),
                Name = "Herschel supply",
                Description = "Premium Herschel backpack",
                Price = 63.16m,
                StockQuantity = 20,
                ImageUrl = "/images/product-11.jpg",
                CategoryId = menCategory.Id,
                IsActive = true,
                CreatedDate = DateTime.UtcNow
            },
            new Product
            {
                Id = Guid.NewGuid(),
                Name = "Herschel supply",
                Description = "Quality Herschel supply item",
                Price = 63.15m,
                StockQuantity = 22,
                ImageUrl = "/images/product-12.jpg",
                CategoryId = menCategory.Id,
                IsActive = true,
                CreatedDate = DateTime.UtcNow
            },

            // Watches
            new Product
            {
                Id = Guid.NewGuid(),
                Name = "Vintage Inspired Classic",
                Description = "Vintage inspired classic timepiece",
                Price = 93.20m,
                StockQuantity = 15,
                ImageUrl = "/images/product-06.jpg",
                CategoryId = watchesCategory.Id,
                IsActive = true,
                CreatedDate = DateTime.UtcNow
            },
            new Product
            {
                Id = Guid.NewGuid(),
                Name = "Mini Silver Mesh Watch",
                Description = "Elegant mini silver mesh watch",
                Price = 86.85m,
                StockQuantity = 18,
                ImageUrl = "/images/product-15.jpg",
                CategoryId = watchesCategory.Id,
                IsActive = true,
                CreatedDate = DateTime.UtcNow
            },

            // Shoes
            new Product
            {
                Id = Guid.NewGuid(),
                Name = "Converse All Star Hi Plimsolls",
                Description = "Classic Converse All Star high top sneakers",
                Price = 75.00m,
                StockQuantity = 40,
                ImageUrl = "/images/product-09.jpg",
                CategoryId = shoesCategory.Id,
                IsActive = true,
                CreatedDate = DateTime.UtcNow
            }
        };

        await context.Products.AddRangeAsync(products);
        await context.SaveChangesAsync();
    }

    private static async Task SeedBlogPostsAsync(CozaStoreDbContext context, UserManager<ApplicationUser> userManager)
    {
        if (await context.BlogPosts.AnyAsync())
            return;

        // Admin kullanıcısını bul
        var adminUser = await userManager.FindByEmailAsync("admin@cozastore.com");
        if (adminUser == null)
            return; // Admin kullanıcısı yoksa blog gönderisi ekleme

        var blogPosts = new List<BlogPost>
        {
            new BlogPost
            {
                Id = Guid.NewGuid(),
                Title = "8 Inspiring Ways to Wear Dresses in the Winter",
                Content = "Class aptent taciti sociosqu ad litora torquent per conubia nostra, per inceptos himenaeos. Fusce eget dictum tortor. Donec dictum vitae sapien eu varius. Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nunc sit amet est vel orci luctus sollicitudin. Duis eleifend vestibulum justo, varius semper lacus condimentum dictum. Donec pulvinar a magna ut malesuada. In posuere felis diam, vel sodales metus accumsan in. Duis viverra dui eu pharetra pellentesque. Donec a eros leo. Quisque sed ligula vitae lorem efficitur faucibus. Praesent sit amet imperdiet ante. Nulla id tellus auctor, dictum libero a, malesuada nisi. Nulla in porta nibh, id vestibulum ipsum. Praesent dapibus tempus erat quis aliquet. Donec ac purus id sapien condimentum feugiat.\n\nPraesent vel mi bibendum, finibus leo ac, condimentum arcu. Pellentesque sem ex, tristique sit amet suscipit in, mattis imperdiet enim. Integer tempus justo nec velit fringilla, eget eleifend neque blandit. Sed tempor magna sed congue auctor. Mauris eu turpis eget tortor ultricies elementum. Phasellus vel placerat orci, a venenatis justo. Phasellus faucibus venenatis nisl vitae vestibulum. Praesent id nibh arcu. Vivamus sagittis accumsan felis, quis vulputate.",
                ImageUrl = "/images/blog-04.jpg",
                IsPublished = true,
                AuthorId = adminUser.Id,
                IsActive = true,
                CreatedDate = DateTime.UtcNow.AddDays(-10)
            },
            new BlogPost
            {
                Id = Guid.NewGuid(),
                Title = "The Great Big List of Men's Gifts for the Holidays",
                Content = "Class aptent taciti sociosqu ad litora torquent per conubia nostra, per inceptos himenaeos. Fusce eget dictum tortor. Donec dictum vitae sapien eu varius. Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nunc sit amet est vel orci luctus sollicitudin. Duis eleifend vestibulum justo, varius semper lacus condimentum dictum. Donec pulvinar a magna ut malesuada. In posuere felis diam, vel sodales metus accumsan in. Duis viverra dui eu pharetra pellentesque. Donec a eros leo. Quisque sed ligula vitae lorem efficitur faucibus. Praesent sit amet imperdiet ante. Nulla id tellus auctor, dictum libero a, malesuada nisi. Nulla in porta nibh, id vestibulum ipsum. Praesent dapibus tempus erat quis aliquet. Donec ac purus id sapien condimentum feugiat.\n\nPraesent vel mi bibendum, finibus leo ac, condimentum arcu. Pellentesque sem ex, tristique sit amet suscipit in, mattis imperdiet enim. Integer tempus justo nec velit fringilla, eget eleifend neque blandit. Sed tempor magna sed congue auctor. Mauris eu turpis eget tortor ultricies elementum. Phasellus vel placerat orci, a venenatis justo. Phasellus faucibus venenatis nisl vitae vestibulum. Praesent id nibh arcu. Vivamus sagittis accumsan felis, quis vulputate.",
                ImageUrl = "/images/blog-05.jpg",
                IsPublished = true,
                AuthorId = adminUser.Id,
                IsActive = true,
                CreatedDate = DateTime.UtcNow.AddDays(-8)
            },
            new BlogPost
            {
                Id = Guid.NewGuid(),
                Title = "5 Winter-to-Spring Fashion Trends to Try Now",
                Content = "Class aptent taciti sociosqu ad litora torquent per conubia nostra, per inceptos himenaeos. Fusce eget dictum tortor. Donec dictum vitae sapien eu varius. Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nunc sit amet est vel orci luctus sollicitudin. Duis eleifend vestibulum justo, varius semper lacus condimentum dictum. Donec pulvinar a magna ut malesuada. In posuere felis diam, vel sodales metus accumsan in. Duis viverra dui eu pharetra pellentesque. Donec a eros leo. Quisque sed ligula vitae lorem efficitur faucibus. Praesent sit amet imperdiet ante. Nulla id tellus auctor, dictum libero a, malesuada nisi. Nulla in porta nibh, id vestibulum ipsum. Praesent dapibus tempus erat quis aliquet. Donec ac purus id sapien condimentum feugiat.\n\nPraesent vel mi bibendum, finibus leo ac, condimentum arcu. Pellentesque sem ex, tristique sit amet suscipit in, mattis imperdiet enim. Integer tempus justo nec velit fringilla, eget eleifend neque blandit. Sed tempor magna sed congue auctor. Mauris eu turpis eget tortor ultricies elementum. Phasellus vel placerat orci, a venenatis justo. Phasellus faucibus venenatis nisl vitae vestibulum. Praesent id nibh arcu. Vivamus sagittis accumsan felis, quis vulputate.",
                ImageUrl = "/images/blog-06.jpg",
                IsPublished = true,
                AuthorId = adminUser.Id,
                IsActive = true,
                CreatedDate = DateTime.UtcNow.AddDays(-5)
            }
        };

        await context.BlogPosts.AddRangeAsync(blogPosts);
        await context.SaveChangesAsync();
    }
}

