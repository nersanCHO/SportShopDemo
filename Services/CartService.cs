using System.Text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using SportShop.Data;
using SportShop.Models;

namespace SportShop.Services;

public class CartService
{
    private readonly ApplicationDbContext _db;
    private readonly IWebHostEnvironment _env;

    public CartService(ApplicationDbContext db, IWebHostEnvironment env)
    {
        _db = db;
        _env = env;
    }

    public async Task<List<CartItem>> GetUserCartItemsAsync(string userId)
    {
        return await _db.CartItems
            .Include(c => c.Product)
            .Where(c => c.UserId == userId)
            .ToListAsync();
    }

    public decimal GetCartTotal(IEnumerable<CartItem> items)
    {
        return items.Sum(i => (i.Product?.Price ?? 0m) * i.Quantity);
    }

    public async Task<ServiceResult> AddAsync(string userId, int productId, string? selectedSize)
    {
        var product = await _db.Products.FirstOrDefaultAsync(p => p.Id == productId);
        if (product == null)
        {
            return ServiceResult.Missing();
        }

        var normalizedSize = Product.NormalizeSingleSize(product.SizeType, selectedSize);

        if (string.IsNullOrWhiteSpace(normalizedSize) || !product.HasAvailableSize(normalizedSize))
        {
            return ServiceResult.Fail("Моля, избери валиден размер.");
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
        return ServiceResult.Ok();
    }

    public async Task DecreaseAsync(string userId, int cartItemId)
    {
        var item = await _db.CartItems
            .FirstOrDefaultAsync(x => x.Id == cartItemId && x.UserId == userId);

        if (item == null)
        {
            return;
        }

        item.Quantity -= 1;

        if (item.Quantity <= 0)
        {
            _db.CartItems.Remove(item);
        }

        await _db.SaveChangesAsync();
    }

    public async Task RemoveAsync(string userId, int cartItemId)
    {
        var item = await _db.CartItems
            .FirstOrDefaultAsync(x => x.Id == cartItemId && x.UserId == userId);

        if (item == null)
        {
            return;
        }

        _db.CartItems.Remove(item);
        await _db.SaveChangesAsync();
    }

    public async Task<ServiceResult> CheckoutAsync(
        string userId,
        string? userEmail,
        string? userName,
        OrderInputModel input)
    {
        var items = await _db.CartItems
            .Include(c => c.Product)
            .Where(c => c.UserId == userId)
            .ToListAsync();

        if (!items.Any())
        {
            return ServiceResult.Fail("Количката е празна.");
        }

        var total = GetCartTotal(items);

        var sb = new StringBuilder();
        sb.AppendLine("SportShop - Нова поръчка (демо)");
        sb.AppendLine();
        sb.AppendLine($"Клиент: {input.FullName}");
        sb.AppendLine($"Имейл: {userEmail}");
        sb.AppendLine($"Телефон: {input.Phone}");
        sb.AppendLine($"Адрес: {input.Address}");
        sb.AppendLine();
        sb.AppendLine("Артикули:");

        foreach (var item in items)
        {
            var productName = item.Product?.Name ?? "N/A";
            var price = item.Product?.Price ?? 0m;

            sb.AppendLine(
                $"{item.Quantity} x {productName} | Размер: {item.SelectedSize} | {price:0.00} € = {(price * item.Quantity):0.00} €");
        }

        sb.AppendLine();
        sb.AppendLine($"Крайна сума: {total:0.00} €");
        sb.AppendLine();
        sb.AppendLine("Бележка: Това е демо checkout. Не се изпраща реален имейл.");

        try
        {
            var webRoot = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            var ordersDir = Path.Combine(webRoot, "orders");
            Directory.CreateDirectory(ordersDir);

            var safeName = string.IsNullOrWhiteSpace(userName)
                ? "guest"
                : userName.Replace(" ", "_");

            var fileName = $"order_{DateTime.UtcNow:yyyyMMdd_HHmmss}_{safeName}.txt";
            var filePath = Path.Combine(ordersDir, fileName);

            await File.WriteAllTextAsync(filePath, sb.ToString(), Encoding.UTF8);

            _db.CartItems.RemoveRange(items);
            await _db.SaveChangesAsync();

            return ServiceResult.Ok("Поръчката е изпратена. Файлът е записан в wwwroot/orders като .txt (демо).");
        }
        catch (Exception ex)
        {
            return ServiceResult.Fail("Грешка при записването на поръчката: " + ex.Message);
        }
    }
}