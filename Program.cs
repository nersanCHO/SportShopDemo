using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SportShop.Data;
using SportShop.Models;
using SportShop.Services;

var builder = WebApplication.CreateBuilder(args);

// Register MVC controllers with views.
builder.Services.AddControllersWithViews();

// Register Razor Pages so the default ASP.NET Core Identity UI
// in Areas/Identity is available.
builder.Services.AddRazorPages();

// Resolve the database connection string from configuration.
// If no connection string is configured, fall back to LocalDB
// for easier local development.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrWhiteSpace(connectionString))
{
    connectionString =
        "Server=(localdb)\\MSSQLLocalDB;Database=SportShop;Trusted_Connection=True;MultipleActiveResultSets=true";
}

// Register the EF Core DbContext.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// Configure ASP.NET Core Identity.
builder.Services
    .AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        options.Password.RequiredLength = 6;
        options.Password.RequireNonAlphanumeric = false;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders()
    .AddDefaultUI();

// Register application services.
builder.Services.AddScoped<ProductCatalogService>();
builder.Services.AddScoped<FavoritesService>();
builder.Services.AddScoped<CartService>();
builder.Services.AddScoped<AdminProductService>();
builder.Services.AddScoped<OrderEmailService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Seed roles, users, and demo data needed by the application.
using (var scope = app.Services.CreateScope())
{
    await SeedData.InitializeAsync(scope.ServiceProvider);
}

// Configure default MVC route.
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Products}/{action=Index}/{id?}");

// Map Identity UI pages.
app.MapRazorPages();

app.Run();