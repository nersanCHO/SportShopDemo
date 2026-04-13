using System.ComponentModel.DataAnnotations;
using System.Text;
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
    private readonly IWebHostEnvironment _env;

    public CartController(
        ApplicationDbContext db,
        UserManager<ApplicationUser> userManager,
        IWebHostEnvironment env)
    {
        _db = db;
        _userManager = userManager;
        _env = env;
    }

    public async Task<IActionResult> Index()
    {
        var userId = _userManager.GetUserId(User)!;

        var items = await _db.CartItems
            .Include(c => c.Product)
            .Where(c => c.UserId == userId)
            .ToListAsync();

        ViewBag.Total = items.Sum(i => (i.Product?.Price ?? 0m) * i.Quantity);

        var favoriteIds = await _db.Favorites
            .Where(f => f.UserId == userId)
            .Select(f => f.ProductId)
            .ToListAsync();

        ViewBag.FavoriteIds = favoriteIds;

        return View(items);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(int productId, string? selectedSize)
    {
        var userId = _userManager.GetUserId(User)!;

        var product = await _db.Products.FirstOrDefaultAsync(p => p.Id == productId);
        if (product == null)
        {
            return NotFound();
        }

        var normalizedSize = Product.NormalizeSingleSize(product.SizeType, selectedSize);

        if (string.IsNullOrWhiteSpace(normalizedSize) || !product.HasAvailableSize(normalizedSize))
        {
            TempData["CartMessage"] = "Моля, избери валиден размер.";
            return RedirectToAction("Details", "Products", new { id = productId });
        }

        var item = await _db.CartItems.FirstOrDefaultAsync(x =>
            x.UserId == userId &&
            x.ProductId == productId &&
            x.SelectedSize == normalizedSize);

        if (item == null)
        {
            _db.CartItems.Add(new CartItem
            {
                UserId = userId,
                ProductId = productId,
                SelectedSize = normalizedSize,
                Quantity = 1
            });
        }
        else
        {
            item.Quantity += 1;
        }

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

            if (item.Quantity <= 0)
            {
                _db.CartItems.Remove(item);
            }

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

    public class OrderInputModel
    {
        [Required]
        public string FullName { get; set; } = string.Empty;

        [Required]
        public string Address { get; set; } = string.Empty;

        [Required]
        public string Phone { get; set; } = string.Empty;
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Checkout(OrderInputModel input)
    {
        if (!ModelState.IsValid)
        {
            TempData["OrderMessage"] = "Моля, попълни всички полета за поръчката.";
            return RedirectToAction(nameof(Index));
        }

        var user = await _userManager.GetUserAsync(User);
        var userId = _userManager.GetUserId(User)!;

        var items = await _db.CartItems
            .Include(c => c.Product)
            .Where(c => c.UserId == userId)
            .ToListAsync();

        if (!items.Any())
        {
            TempData["OrderMessage"] = "Количката е празна.";
            return RedirectToAction(nameof(Index));
        }

        var total = items.Sum(i => (i.Product?.Price ?? 0m) * i.Quantity);

        var sb = new StringBuilder();
        sb.AppendLine("SportShop - Нова поръчка (демо)");
        sb.AppendLine();
        sb.AppendLine($"Клиент: {input.FullName}");
        sb.AppendLine($"Имейл: {user?.Email}");
        sb.AppendLine($"Телефон: {input.Phone}");
        sb.AppendLine($"Адрес: {input.Address}");
        sb.AppendLine();
        sb.AppendLine("Артикули:");

        foreach (var it in items)
        {
            var name = it.Product?.Name ?? "N/A";
            var price = it.Product?.Price ?? 0m;

            sb.AppendLine(
                $"{it.Quantity} x {name} | Размер: {it.SelectedSize} | {price:0.00} € = {(price * it.Quantity):0.00} €");
        }

        sb.AppendLine();
        sb.AppendLine($"Крайна сума: {total:0.00} €");
        sb.AppendLine();
        sb.AppendLine("Бележка: Това е демо checkout. Не се изпраща реален имейл.");

        try
        {
            var ordersDir = Path.Combine(
                _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"),
                "orders");

            if (!Directory.Exists(ordersDir))
            {
                Directory.CreateDirectory(ordersDir);
            }

            var safeName = string.IsNullOrWhiteSpace(user?.UserName)
                ? "guest"
                : user!.UserName!.Replace(" ", "_");

            var fileName = $"order_{DateTime.UtcNow:yyyyMMdd_HHmmss}_{safeName}.txt";
            var filePath = Path.Combine(ordersDir, fileName);

            await System.IO.File.WriteAllTextAsync(filePath, sb.ToString(), Encoding.UTF8);

            _db.CartItems.RemoveRange(items);
            await _db.SaveChangesAsync();

            TempData["OrderMessage"] = "Поръчката е изпратена. Файлът е записан в wwwroot/orders като .txt (демо).";
        }
        catch (Exception ex)
        {
            TempData["OrderMessage"] = "Грешка при записването на поръчката: " + ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }
}