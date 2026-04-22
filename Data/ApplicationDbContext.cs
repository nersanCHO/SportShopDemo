using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SportShop.Models;

namespace SportShop.Data;

/// <summary>
/// Main Entity Framework Core database context for the SportShop application.
/// It combines ASP.NET Core Identity tables with the shop domain entities.
/// </summary>
public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    /// <summary>
    /// Creates a new database context instance using the configured options.
    /// </summary>
    /// <param name="options">The EF Core options for this context.</param>
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// Products available in the catalog.
    /// </summary>
    public DbSet<Product> Products => Set<Product>();

    /// <summary>
    /// Items added to user shopping carts.
    /// </summary>
    public DbSet<CartItem> CartItems => Set<CartItem>();

    /// <summary>
    /// Products marked as favorites by users.
    /// </summary>
    public DbSet<Favorite> Favorites => Set<Favorite>();

    /// <summary>
    /// Additional photos attached to products.
    /// </summary>
    public DbSet<ProductPhoto> ProductPhotos => Set<ProductPhoto>();

    /// <summary>
    /// Configures entity rules, indexes, and relationships.
    /// </summary>
    /// <param name="builder">The EF Core model builder.</param>
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // A user can favorite a given product only once.
        builder.Entity<Favorite>()
            .HasIndex(f => new { f.UserId, f.ProductId })
            .IsUnique();

        // A user can have only one cart row for the same product/size combination.
        builder.Entity<CartItem>()
            .HasIndex(c => new { c.UserId, c.ProductId, c.SelectedSize })
            .IsUnique();

        builder.Entity<CartItem>()
            .Property(c => c.SelectedSize)
            .HasMaxLength(20);

        builder.Entity<Product>()
            .Property(p => p.AvailableSizes)
            .HasMaxLength(200);

        // When a product is deleted, remove all of its photos as well.
        builder.Entity<ProductPhoto>()
            .HasOne(pp => pp.Product)
            .WithMany(p => p.Photos)
            .HasForeignKey(pp => pp.ProductId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}