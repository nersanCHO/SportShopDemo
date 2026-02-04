using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SportShop.Data;
using SportShop.Models;

namespace SportShop.Controllers;

[Authorize]
public class CartController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public CartController(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var userId = _userManager.GetUserId(User)!;

        var items = await _db.CartItems
            .Include(c => c.Product)
            .Where(c => c.UserId == userId)
            .ToListAsync();

        ViewBag.Total = items.Sum(i => (i.Product?.Price ?? 0m) * i.Quantity);

        // Load favourite product ids for the current user so the view can render a different button state
        var favoriteIds = await _db.Favorites
            .Where(f => f.UserId == userId)
            .Select(f => f.ProductId)
            .ToListAsync();

        ViewBag.FavoriteIds = favoriteIds;

        return View(items);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(int productId)
    {
        var userId = _userManager.GetUserId(User)!;

        var item = await _db.CartItems.FirstOrDefaultAsync(x => x.UserId == userId && x.ProductId == productId);
        if (item == null)
            _db.CartItems.Add(new CartItem { UserId = userId, ProductId = productId, Quantity = 1 });
        else
            item.Quantity += 1;

        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Decrease(int cartItemId)
    {
        var userId = _userManager.GetUserId(User)!;
        var item = await _db.CartItems.FirstOrDefaultAsync(x => x.Id == cartItemId && x.UserId == userId);
        if (item != null)
        {
            item.Quantity -= 1;
            if (item.Quantity <= 0) _db.CartItems.Remove(item);
            await _db.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Remove(int cartItemId)
    {
        var userId = _userManager.GetUserId(User)!;
        var item = await _db.CartItems.FirstOrDefaultAsync(x => x.Id == cartItemId && x.UserId == userId);
        if (item != null)
        {
            _db.CartItems.Remove(item);
            await _db.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }
}
