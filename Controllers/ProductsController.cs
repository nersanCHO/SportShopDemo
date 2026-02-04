using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SportShop.Data;
using SportShop.Models;

namespace SportShop.Controllers;

public class ProductsController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public ProductsController(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index(
        GenderTarget? gender,
        string? sport,
        string? subCategory,
        decimal? minPrice,
        decimal? maxPrice,
        string? q)
    {
        var products = _db.Products.AsQueryable();

        if (!string.IsNullOrWhiteSpace(q))
            products = products.Where(p => p.Name.Contains(q) || (p.Description ?? "").Contains(q));

        if (gender.HasValue)
            products = products.Where(p => p.Gender == gender.Value);

        if (!string.IsNullOrWhiteSpace(sport))
            products = products.Where(p => p.Sport == sport);

        if (!string.IsNullOrWhiteSpace(subCategory))
            products = products.Where(p => p.SubCategory == subCategory);

        if (minPrice.HasValue)
            products = products.Where(p => p.Price >= minPrice.Value);

        if (maxPrice.HasValue)
            products = products.Where(p => p.Price <= maxPrice.Value);

        ViewBag.Sports = await _db.Products.Select(p => p.Sport).Distinct().OrderBy(x => x).ToListAsync();
        ViewBag.SubCategories = await _db.Products.Select(p => p.SubCategory).Distinct().OrderBy(x => x).ToListAsync();

        // Load favourite ids for the current user
        var userId = _userManager.GetUserId(User);
        if (!string.IsNullOrEmpty(userId))
        {
            var favIds = await _db.Favorites.Where(f => f.UserId == userId).Select(f => f.ProductId).ToListAsync();
            ViewBag.FavoriteIds = favIds;
        }
        else
        {
            ViewBag.FavoriteIds = new List<int>();
        }

        // include photos so views can pick first photo
        products = products.Include(p => p.Photos);

        return View(await products.OrderBy(p => p.Name).ToListAsync());
    }

    public async Task<IActionResult> Details(int id)
    {
        var product = await _db.Products.Include(p => p.Photos).FirstOrDefaultAsync(p => p.Id == id);
        if (product == null) return NotFound();

        var userId = _userManager.GetUserId(User);
        if (!string.IsNullOrEmpty(userId))
        {
            var exists = await _db.Favorites.AnyAsync(f => f.UserId == userId && f.ProductId == id);
            ViewBag.IsFavorite = exists;
        }
        else
        {
            ViewBag.IsFavorite = false;
        }

        return View(product);
    }
}
