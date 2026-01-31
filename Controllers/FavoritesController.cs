using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SportShop.Data;
using SportShop.Models;

namespace SportShop.Controllers;

[Authorize]
public class FavoritesController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public FavoritesController(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var userId = _userManager.GetUserId(User)!;

        var favorites = await _db.Favorites
            .Include(f => f.Product)
            .Where(f => f.UserId == userId)
            .Select(f => f.Product!)
            .ToListAsync();

        return View(favorites);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Toggle(int productId)
    {
        var userId = _userManager.GetUserId(User)!;

        var existing = await _db.Favorites.FirstOrDefaultAsync(f => f.UserId == userId && f.ProductId == productId);
        if (existing == null)
            _db.Favorites.Add(new Favorite { UserId = userId, ProductId = productId });
        else
            _db.Favorites.Remove(existing);

        await _db.SaveChangesAsync();
        return RedirectToAction("Details", "Products", new { id = productId });
    }
}
