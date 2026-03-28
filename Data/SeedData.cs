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
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
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
            {
                await userManager.AddToRoleAsync(admin, "Admin");
            }
        }

        // Seed products only if they do not already exist.
        // IMPORTANT: do NOT delete and recreate seeded products,
        // because that breaks existing cart/favorites relationships.
        var productsToSeed = new List<Product>
        {
            new Product
            {
                Name = SeedPrefix + "Nike Air Zoom Pegasus 40",
                Sport = "Running",
                SubCategory = "Shoes",
                Price = 129.99m,
                Gender = GenderTarget.Unisex,
                Size = ProductSize.M,
                Description = "Responsive running shoes built for daily training.",
                ImagePath = "/images/NikeAirZoomPegasus40/main.jpg",
                Photos = new List<ProductPhoto>
                {
                    new ProductPhoto { ImagePath = "/images/NikeAirZoomPegasus40/2.jpg" },
                    new ProductPhoto { ImagePath = "/images/NikeAirZoomPegasus40/3.jpg" },
                    new ProductPhoto { ImagePath = "/images/NikeAirZoomPegasus40/4.jpg" }
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
                ImagePath = "/images/AdidasPredatorEdge/main.jpg",
                Photos = new List<ProductPhoto>
                {
                    new ProductPhoto { ImagePath = "/images/AdidasPredatorEdge/2.jpg" },
                    new ProductPhoto { ImagePath = "/images/AdidasPredatorEdge/3.jpg" },
                    new ProductPhoto { ImagePath = "/images/AdidasPredatorEdge/4.jpg" }
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
                ImagePath = "/images/TrainingTShirtDryFit/main.jpg",
                Photos = new List<ProductPhoto>
                {
                    new ProductPhoto { ImagePath = "/images/TrainingTShirtDryFit/2.jpg" },
                    new ProductPhoto { ImagePath = "/images/TrainingTShirtDryFit/3.jpg" }
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
                ImagePath = "/images/SportSocksPro/main.jpg",
                Photos = new List<ProductPhoto>
                {
                    new ProductPhoto { ImagePath = "/images/SportSocksPro/2.jpg" }
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
                ImagePath = "/images/OutdoorWindJacket/main.jpg",
                Photos = new List<ProductPhoto>
                {
                    new ProductPhoto { ImagePath = "/images/OutdoorWindJacket/2.jpg" }
                }
            }
        };

        foreach (var product in productsToSeed)
        {
            var exists = await context.Products.AnyAsync(p => p.Name == product.Name);

            if (!exists)
            {
                context.Products.Add(product);
            }
        }

        await context.SaveChangesAsync();
    }
}