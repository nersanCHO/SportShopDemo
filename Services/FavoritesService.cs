using Microsoft.EntityFrameworkCore;
using SportShop.Data;
using SportShop.Models;

namespace SportShop.Services;

/// <summary>
/// Encapsulates favorite-product operations for authenticated users.
/// </summary>
public class FavoritesService
{
    private readonly ApplicationDbContext _db;

    /// <summary>
    /// Creates the service with a database context dependency.
    /// </summary>
    public FavoritesService(ApplicationDbContext db)
    {
        _db = db;
    }

    /// <summary>
    /// Returns all favorite products for the specified user.
    /// </summary>
    public async Task<List<Product>> GetUserFavoritesAsync(string userId)
    {
        return await _db.Favorites
            .Include(f => f.Product)
            .Where(f => f.UserId == userId)
            .Select(f => f.Product!)
            .ToListAsync();
    }

    /// <summary>
    /// Returns only the favorite product IDs for the specified user.
    /// </summary>
    public async Task<List<int>> GetFavoriteProductIdsAsync(string userId)
    {
        return await _db.Favorites
            .Where(f => f.UserId == userId)
            .Select(f => f.ProductId)
            .ToListAsync();
    }

    /// <summary>
    /// Checks whether a given product is currently in the user's favorites.
    /// </summary>
    public async Task<bool> IsFavoriteAsync(string userId, int productId)
    {
        return await _db.Favorites
            .AnyAsync(f => f.UserId == userId && f.ProductId == productId);
    }

    /// <summary>
    /// Adds a favorite if it does not exist, otherwise removes it.
    /// </summary>
    public async Task ToggleAsync(string userId, int productId)
    {
        var existing = await _db.Favorites
            .FirstOrDefaultAsync(f => f.UserId == userId && f.ProductId == productId);

        if (existing == null)
        {
            _db.Favorites.Add(new Favorite
            {
                UserId = userId,
                ProductId = productId
            });
        }
        else
        {
            _db.Favorites.Remove(existing);
        }

        await _db.SaveChangesAsync();
    }
}