
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SportShop.Models;

namespace SportShop.Data;

public static class SeedData
{
    public static async Task InitializeAsync(IServiceProvider services)
    {
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var context = services.GetRequiredService<ApplicationDbContext>();

        await context.Database.MigrateAsync();

        string[] roles = { "Admin", "Client" };
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));
        }

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

        if (!context.Products.Any())
        {
            context.Products.AddRange(
                new Product
                {
                    Name = "Nike Air Zoom Pegasus 40",
                    Sport = "Running",
                    SubCategory = "Shoes",
                    Price = 129.99m,
                    Gender = GenderTarget.Unisex,
                    Description = "Responsive running shoes built for daily training.",
                    ImagePath = "/images/default.png"
                },
                new Product
                {
                    Name = "Adidas Predator Edge",
                    Sport = "Football",
                    SubCategory = "Cleats",
                    Price = 199.99m,
                    Gender = GenderTarget.Men,
                    Description = "Precision football boots for maximum control.",
                    ImagePath = "/images/default.png"
                }
            );

            await context.SaveChangesAsync();
        }
    }
}
