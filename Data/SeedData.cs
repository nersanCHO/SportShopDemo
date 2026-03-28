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

        await context.Database.MigrateAsync();

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
        else
        {
            if (!await userManager.IsInRoleAsync(admin, "Admin"))
            {
                await userManager.AddToRoleAsync(admin, "Admin");
            }
        }

        // Remove old seeded products only
        var seededProducts = await context.Products
            .Include(p => p.Photos)
            .Where(p => p.Name.StartsWith(SeedPrefix))
            .ToListAsync();

        if (seededProducts.Count > 0)
        {
            var seededProductIds = seededProducts.Select(p => p.Id).ToList();

            var seededCartItems = await context.CartItems
                .Where(c => seededProductIds.Contains(c.ProductId))
                .ToListAsync();

            if (seededCartItems.Count > 0)
            {
                context.CartItems.RemoveRange(seededCartItems);
            }

            var seededFavorites = await context.Favorites
                .Where(f => seededProductIds.Contains(f.ProductId))
                .ToListAsync();

            if (seededFavorites.Count > 0)
            {
                context.Favorites.RemoveRange(seededFavorites);
            }

            var seededPhotos = seededProducts
                .SelectMany(p => p.Photos)
                .ToList();

            if (seededPhotos.Count > 0)
            {
                context.ProductPhotos.RemoveRange(seededPhotos);
            }

            context.Products.RemoveRange(seededProducts);
            await context.SaveChangesAsync();
        }

        context.Products.AddRange(
            new Product
            {
                Name = SeedPrefix + "Nike Air Zoom Pegasus 40",
                Sport = "Running",
                SubCategory = "Shoes",
                Price = 129.99m,
                Gender = GenderTarget.Unisex,
                SizeType = ProductSizeType.Shoes,
                AvailableSizes = Product.NormalizeSizeList(ProductSizeType.Shoes, "39,40,41,42,43,44,45"),
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
                SizeType = ProductSizeType.Shoes,
                AvailableSizes = Product.NormalizeSizeList(ProductSizeType.Shoes, "40,41,42,43,44,45,46"),
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
                SizeType = ProductSizeType.Clothing,
                AvailableSizes = Product.NormalizeSizeList(ProductSizeType.Clothing, "XS,S,M,L,XL"),
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
                SizeType = ProductSizeType.Universal,
                AvailableSizes = Product.NormalizeSizeList(ProductSizeType.Universal, "Universal"),
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
                SizeType = ProductSizeType.Clothing,
                AvailableSizes = Product.NormalizeSizeList(ProductSizeType.Clothing, "M,L,XL"),
                Description = "Lightweight jacket for windy weather.",
                ImagePath = "/images/OutdoorWindJacket/main.jpg",
                Photos = new List<ProductPhoto>
                {
                    new ProductPhoto { ImagePath = "/images/OutdoorWindJacket.jpg" }
                }
            }
        );

        await context.SaveChangesAsync();
    }
}