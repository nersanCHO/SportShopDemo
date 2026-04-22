using Microsoft.EntityFrameworkCore;
using SportShop.Data;
using SportShop.Models;

namespace SportShop.Services;

public class FavoritesService
{
    private readonly ApplicationDbContext _db;

    public FavoritesService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<List<Product>> GetUserFavoritesAsync(string userId)
    {
        return await _db.Favorites
            .Include(f => f.Product)
            .Where(f => f.UserId == userId)
            .Select(f => f.Product!)
            .ToListAsync();
    }

    public async Task<List<int>> GetFavoriteProductIdsAsync(string userId)
    {
        return await _db.Favorites
            .Where(f => f.UserId == userId)
            .Select(f => f.ProductId)
            .ToListAsync();
    }

    public async Task<bool> IsFavoriteAsync(string userId, int productId)
    {
        return await _db.Favorites
            .AnyAsync(f => f.UserId == userId && f.ProductId == productId);
    }

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