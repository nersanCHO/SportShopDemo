using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SportShop.Data;
using SportShop.Models;
using System.Net.Mail;
using System.Text;

namespace SportShop.Controllers;

[Authorize]
public class CartController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IWebHostEnvironment _env;

    public CartController(ApplicationDbContext db, UserManager<ApplicationUser> userManager, IWebHostEnvironment env)
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

    // Checkout: collects customer info, sends order email (writes .eml to wwwroot/orders for demo),
    // and clears the cart.
    public class OrderInputModel
    {
        public string FullName { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Checkout(OrderInputModel input)
    {
        if (!ModelState.IsValid)
            return RedirectToAction(nameof(Index));

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

        // Build email body
        var sb = new StringBuilder();
        sb.AppendLine("SportShop - Ново поръчване (демо)");
        sb.AppendLine();
        sb.AppendLine($"Поръчител: {input.FullName}");
        sb.AppendLine($"Имейл: {user?.Email}");
        sb.AppendLine($"Телефон: {input.Phone}");
        sb.AppendLine($"Адрес: {input.Address}");
        sb.AppendLine();
        sb.AppendLine("Артикули:");

        foreach (var it in items)
        {
            var name = it.Product?.Name ?? "N/A";
            var price = it.Product?.Price ?? 0m;
            sb.AppendLine($"{it.Quantity} x {name} @ {price:0.00} лв. = {(price * it.Quantity):0.00} лв.");
        }

        sb.AppendLine();
        sb.AppendLine($"Междинна сума: {total:0.00} лв.");
        sb.AppendLine();
        sb.AppendLine("Бележка: Това е демонстрационен checkout. Не се извършват реални плащания.");

        try
        {
            // For a demo-friendly approach write an .eml file to wwwroot/orders so you can inspect it.
            var ordersDir = Path.Combine(_env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"), "orders");
            if (!Directory.Exists(ordersDir))
                Directory.CreateDirectory(ordersDir);

            var mail = new MailMessage();
            mail.From = new MailAddress("no-reply@sportshop.local", "SportShop (demo)");
            // send to the user and also to a local admin copy
            if (!string.IsNullOrWhiteSpace(user?.Email))
            {
                mail.To.Add(new MailAddress(user.Email));
            }
            mail.To.Add(new MailAddress("orders@sportshop.local"));
            mail.Subject = $"Поръчка от {input.FullName} - {DateTime.UtcNow:yyyy-MM-dd HH:mm} (demo)";
            mail.Body = sb.ToString();

            using var smtp = new SmtpClient();
            smtp.DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory;
            smtp.PickupDirectoryLocation = ordersDir;
            smtp.Send(mail);

            // Clear cart items
            _db.CartItems.RemoveRange(items);
            await _db.SaveChangesAsync();

            TempData["OrderMessage"] = "Поръчката е получена. Провери имейла си или папката wwwroot/orders (демо).";
        }
        catch (Exception ex)
        {
            // Log if you have logger; for now return friendly message.
            TempData["OrderMessage"] = "Грешка при изпращане на поръчката: " + ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }
}
