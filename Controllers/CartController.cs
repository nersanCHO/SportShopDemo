using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SportShop.Models;
using SportShop.Services;

namespace SportShop.Controllers;

[Authorize]
public class CartController : Controller
{
    private readonly CartService _cartService;
    private readonly FavoritesService _favoritesService;
    private readonly UserManager<ApplicationUser> _userManager;

    public CartController(
        CartService cartService,
        FavoritesService favoritesService,
        UserManager<ApplicationUser> userManager)
    {
        _cartService = cartService;
        _favoritesService = favoritesService;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var userId = _userManager.GetUserId(User)!;

        var items = await _cartService.GetUserCartItemsAsync(userId);
        ViewBag.Total = _cartService.GetCartTotal(items);
        ViewBag.FavoriteIds = await _favoritesService.GetFavoriteProductIdsAsync(userId);

        return View(items);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(int productId, string? selectedSize)
    {
        var userId = _userManager.GetUserId(User)!;
        var result = await _cartService.AddAsync(userId, productId, selectedSize);

        if (result.NotFound)
        {
            return NotFound();
        }

        if (!result.Success)
        {
            TempData["CartMessage"] = result.Message;
            return RedirectToAction("Details", "Products", new { id = productId });
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Decrease(int cartItemId)
    {
        var userId = _userManager.GetUserId(User)!;
        await _cartService.DecreaseAsync(userId, cartItemId);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Remove(int cartItemId)
    {
        var userId = _userManager.GetUserId(User)!;
        await _cartService.RemoveAsync(userId, cartItemId);
        return RedirectToAction(nameof(Index));
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

        var userId = _userManager.GetUserId(User)!;
        var user = await _userManager.GetUserAsync(User);

        var result = await _cartService.CheckoutAsync(userId, user?.Email, user?.UserName, input);
        TempData["OrderMessage"] = result.Message;

        return RedirectToAction(nameof(Index));
    }
}