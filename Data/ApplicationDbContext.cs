using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SportShop.Models;

namespace SportShop.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Product> Products => Set<Product>();
    public DbSet<CartItem> CartItems => Set<CartItem>();
    public DbSet<Favorite> Favorites => Set<Favorite>();
    public DbSet<ProductPhoto> ProductPhotos => Set<ProductPhoto>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Favorite>()
            .HasIndex(f => new { f.UserId, f.ProductId })
            .IsUnique();

        builder.Entity<CartItem>()
            .HasIndex(c => new { c.UserId, c.ProductId, c.SelectedSize })
            .IsUnique();

        builder.Entity<CartItem>()
            .Property(c => c.SelectedSize)
            .HasMaxLength(20);

        builder.Entity<Product>()
            .Property(p => p.AvailableSizes)
            .HasMaxLength(200);

        builder.Entity<ProductPhoto>()
            .HasOne(pp => pp.Product)
            .WithMany(p => p.Photos)
            .HasForeignKey(pp => pp.ProductId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}