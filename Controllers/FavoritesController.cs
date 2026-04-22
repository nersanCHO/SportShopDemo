using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SportShop.Models;
using SportShop.Services;

namespace SportShop.Controllers;

/// <summary>
/// Manages the authenticated user's favorites list.
/// </summary>
[Authorize]
public class FavoritesController : Controller
{
    private readonly FavoritesService _favoritesService;
    private readonly UserManager<ApplicationUser> _userManager;

    /// <summary>
    /// Creates the controller with the required dependencies.
    /// </summary>
    public FavoritesController(
        FavoritesService favoritesService,
        UserManager<ApplicationUser> userManager)
    {
        _favoritesService = favoritesService;
        _userManager = userManager;
    }

    /// <summary>
    /// Displays the current user's favorite products.
    /// </summary>
    public async Task<IActionResult> Index()
    {
        var userId = _userManager.GetUserId(User)!;
        var favorites = await _favoritesService.GetUserFavoritesAsync(userId);
        return View(favorites);
    }

    /// <summary>
    /// Toggles a product in or out of the user's favorites.
    /// </summary>
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