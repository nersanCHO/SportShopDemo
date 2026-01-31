using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SportShop.Data;
using SportShop.Models;

namespace SportShop.Controllers;

public class ProductsController : Controller
{
    private readonly ApplicationDbContext _db;
    public ProductsController(ApplicationDbContext db) => _db = db;

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

        return View(await products.OrderBy(p => p.Name).ToListAsync());
    }

    public async Task<IActionResult> Details(int id)
    {
        var product = await _db.Products.FirstOrDefaultAsync(p => p.Id == id);
        if (product == null) return NotFound();
        return View(product);
    }
}
