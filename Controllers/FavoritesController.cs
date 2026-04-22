using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SportShop.Models;
using SportShop.Services;

namespace SportShop.Controllers;

[Authorize]
public class FavoritesController : Controller
{
    private readonly FavoritesService _favoritesService;
    private readonly UserManager<ApplicationUser> _userManager;

    public FavoritesController(
        FavoritesService favoritesService,
        UserManager<ApplicationUser> userManager)
    {
        _favoritesService = favoritesService;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var userId = _userManager.GetUserId(User)!;
        var favorites = await _favoritesService.GetUserFavoritesAsync(userId);
        return View(favorites);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Toggle(int productId, string? returnUrl = null)
    {
        var userId = _userManager.GetUserId(User)!;
        await _favoritesService.ToggleAsync(userId, productId);

        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }

        return RedirectToAction("Details", "Products", new { id = productId });
    }
}