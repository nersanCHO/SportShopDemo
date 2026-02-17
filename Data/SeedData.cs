using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SportShop.Models;

namespace SportShop.Data;

public static class SeedData
{
    private const string SeedPrefix = "[SEED] ";

    public static async Task InitializeAsync(IServiceProvider services)
    {
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var context = services.GetRequiredService<ApplicationDbContext>();

       

        // Roles
        string[] roles = { "Admin", "Client" };
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));
        }

        // Admin user
        var adminEmail = "admin@sportshop.com";
        var admin = await userManager.FindByEmailAsync(adminEmail);
        if (admin == null)
        {
            admin = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(admin, "Admin123!");
            if (result.Succeeded)
                await userManager.AddToRoleAsync(admin, "Admin");
        }

        // ✅ Reseed only SeedData products (leave user-created products untouched)
        var seededProducts = await context.Products
            .Include(p => p.Photos)
            .Where(p => p.Name.StartsWith(SeedPrefix))
            .ToListAsync();

        if (seededProducts.Count > 0)
        {
            // Remove photos first (safe even if cascade exists)
            var seededPhotos = seededProducts.SelectMany(p => p.Photos).ToList();
            if (seededPhotos.Count > 0)
                context.ProductPhotos.RemoveRange(seededPhotos);

            context.Products.RemoveRange(seededProducts);
            await context.SaveChangesAsync();
        }

        // ✅ Add seed products (6), each has 2+ Photos (not counting main ImagePath)
        context.Products.AddRange(
            new Product
            {
                Name = SeedPrefix + "Nike Air Zoom Pegasus 40",
                Sport = "Running",
                SubCategory = "Shoes",
                Price = 129.99m,
                Gender = GenderTarget.Unisex,
                Size = ProductSize.M,
                Description = "Responsive running shoes built for daily training.",
                ImagePath = "/images/products/HM6803-101_01.jpg",
                Photos = new List<ProductPhoto>
                {
                    new ProductPhoto { ImagePath = "/images/products/running-shoe-profile-9097098.jpg" },
                    new ProductPhoto { ImagePath = "/images/products/shoe-3059711.jpg" }
                }
            },
            new Product
            {
                Name = SeedPrefix + "Adidas Predator Edge",
                Sport = "Football",
                SubCategory = "Cleats",
                Price = 199.99m,
                Gender = GenderTarget.Men,
                Size = ProductSize.L,
                Description = "Precision football boots for maximum control.",
                ImagePath = "/images/products/shoe-3059711.jpg",
                Photos = new List<ProductPhoto>
                {
                    new ProductPhoto { ImagePath = "/images/products/HM6803-101_01.jpg" },
                    new ProductPhoto { ImagePath = "/images/products/running-shoe-profile-9097098.jpg" }
                }
            },
            new Product
            {
                Name = SeedPrefix + "Training T-Shirt DryFit",
                Sport = "Fitness",
                SubCategory = "Apparel",
                Price = 24.99m,
                Gender = GenderTarget.Unisex,
                Size = ProductSize.S,
                Description = "Lightweight training tee for workouts.",
                ImagePath = "/images/products/a2369736-01bc-4edc-9a09-753859dc15d0.jpg",
                Photos = new List<ProductPhoto>
                {
                    new ProductPhoto { ImagePath = "/images/products/default.png" },
                    new ProductPhoto { ImagePath = "/images/products/child-hands-tie-up-shoe-laces-27875976.jpg" }
                }
            },
            new Product
            {
                Name = SeedPrefix + "Compression Leggings",
                Sport = "Fitness",
                SubCategory = "Apparel",
                Price = 39.99m,
                Gender = GenderTarget.Women,
                Size = ProductSize.M,
                Description = "Comfortable compression leggings.",
                ImagePath = "/images/products/running-shoe-profile-9097098.jpg",
                Photos = new List<ProductPhoto>
                {
                    new ProductPhoto { ImagePath = "/images/products/a2369736-01bc-4edc-9a09-753859dc15d0.jpg" },
                    new ProductPhoto { ImagePath = "/images/products/default.png" }
                }
            },
            new Product
            {
                Name = SeedPrefix + "Sport Socks Pro",
                Sport = "Running",
                SubCategory = "Accessories",
                Price = 14.99m,
                Gender = GenderTarget.Unisex,
                Size = ProductSize.Universal,
                Description = "Breathable socks for everyday running.",
                ImagePath = "/images/products/child-hands-tie-up-shoe-laces-27875976.jpg",
                Photos = new List<ProductPhoto>
                {
                    new ProductPhoto { ImagePath = "/images/products/running-shoe-profile-9097098.jpg" },
                    new ProductPhoto { ImagePath = "/images/products/HM6803-101_01.jpg" }
                }
            },
            new Product
            {
                Name = SeedPrefix + "Outdoor Wind Jacket",
                Sport = "Outdoor",
                SubCategory = "Apparel",
                Price = 59.99m,
                Gender = GenderTarget.Men,
                Size = ProductSize.XL,
                Description = "Lightweight jacket for windy weather.",
                ImagePath = "/images/products/default.png",
                Photos = new List<ProductPhoto>
                {
                    new ProductPhoto { ImagePath = "/images/products/a2369736-01bc-4edc-9a09-753859dc15d0.jpg" },
                    new ProductPhoto { ImagePath = "/images/products/shoe-3059711.jpg" }
                }
            }
        );

        await context.SaveChangesAsync();
    }
}