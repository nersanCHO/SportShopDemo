using Microsoft.EntityFrameworkCore;
using SportShop.Data;
using SportShop.Models;

namespace SportShop.Services;

public class CartService
{
    private readonly ApplicationDbContext _db;
    private readonly OrderEmailService _orderEmailService;

    public CartService(
        ApplicationDbContext db,
        OrderEmailService orderEmailService)
    {
        _db = db;
        _orderEmailService = orderEmailService;
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
        if (string.IsNullOrWhiteSpace(userEmail))
        {
            return ServiceResult.Fail("Няма намерен имейл за този акаунт.");
        }

        var items = await _db.CartItems
            .Include(c => c.Product)
            .Where(c => c.UserId == userId)
            .ToListAsync();

        if (!items.Any())
        {
            return ServiceResult.Fail("Количката е празна.");
        }

        var total = GetCartTotal(items);

        try
        {
            await _orderEmailService.SendOrderConfirmationAsync(
                toEmail: userEmail,
                customerName: input.FullName,
                address: input.Address,
                phone: input.Phone,
                items: items,
                total: total);

            _db.CartItems.RemoveRange(items);
            await _db.SaveChangesAsync();

            return ServiceResult.Ok(
                $"Поръчката е изпратена успешно. Имейл за потвърждение беше изпратен до {userEmail}.");
        }
        catch (Exception ex)
        {
            return ServiceResult.Fail("Грешка при изпращане на имейла: " + ex.Message);
        }
    }
}