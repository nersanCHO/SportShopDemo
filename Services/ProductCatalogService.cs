using Microsoft.EntityFrameworkCore;
using SportShop.Data;
using SportShop.Models;

namespace SportShop.Services;

/// <summary>
/// Provides read-only catalog functionality such as filtering products,
/// loading distinct categories, and retrieving product details.
/// </summary>
public class ProductCatalogService
{
    private readonly ApplicationDbContext _db;

    /// <summary>
    /// Creates the service with a database context dependency.
    /// </summary>
    public ProductCatalogService(ApplicationDbContext db)
    {
        _db = db;
    }

    /// <summary>
    /// Returns the product list filtered by the optional search parameters.
    /// </summary>
    public async Task<List<Product>> GetFilteredProductsAsync(
        GenderTarget? gender,
        string? sport,
        string? subCategory,
        decimal? minPrice,
        decimal? maxPrice,
        string? q)
    {
        var products = _db.Products.AsQueryable();

        if (!string.IsNullOrWhiteSpace(q))
        {
            products = products.Where(p =>
                p.Name.Contains(q) ||
                (p.Description ?? string.Empty).Contains(q));
        }

        if (gender.HasValue)
        {
            products = products.Where(p => p.Gender == gender.Value);
        }

        if (!string.IsNullOrWhiteSpace(sport))
        {
            products = products.Where(p => p.Sport == sport);
        }

        if (!string.IsNullOrWhiteSpace(subCategory))
        {
            products = products.Where(p => p.SubCategory == subCategory);
        }

        if (minPrice.HasValue)
        {
            products = products.Where(p => p.Price >= minPrice.Value);
        }

        if (maxPrice.HasValue)
        {
            products = products.Where(p => p.Price <= maxPrice.Value);
        }

        return await products
            .Include(p => p.Photos)
            .OrderBy(p => p.Name)
            .ToListAsync();
    }

    /// <summary>
    /// Returns all distinct sports used in the product catalog.
    /// </summary>
    public async Task<List<string>> GetSportsAsync()
    {
        return await _db.Products
            .Select(p => p.Sport)
            .Distinct()
            .OrderBy(x => x)
            .ToListAsync();
    }

    /// <summary>
    /// Returns all distinct product subcategories.
    /// </summary>
    public async Task<List<string>> GetSubCategoriesAsync()
    {
        return await _db.Products
            .Select(p => p.SubCategory)
            .Distinct()
            .OrderBy(x => x)
            .ToListAsync();
    }

    /// <summary>
    /// Returns a single product by ID including its photos.
    /// </summary>
    public async Task<Product?> GetByIdWithPhotosAsync(int id)
    {
        return await _db.Products
            .Include(p => p.Photos)
            .FirstOrDefaultAsync(p => p.Id == id);
    }
}