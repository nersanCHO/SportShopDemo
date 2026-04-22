using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SportShop.Models;
using SportShop.Services;

namespace SportShop.Controllers;

/// <summary>
/// Handles public product browsing, filtering, and product details.
/// </summary>
public class ProductsController : Controller
{
    private readonly ProductCatalogService _productCatalogService;
    private readonly FavoritesService _favoritesService;
    private readonly UserManager<ApplicationUser> _userManager;

    /// <summary>
    /// Creates the controller with the required services.
    /// </summary>
    public ProductsController(
        ProductCatalogService productCatalogService,
        FavoritesService favoritesService,
        UserManager<ApplicationUser> userManager)
    {
        _productCatalogService = productCatalogService;
        _favoritesService = favoritesService;
        _userManager = userManager;
    }

    /// <summary>
    /// Displays the catalog page with optional filters.
    /// </summary>
    public async Task<IActionResult> Index(
        GenderTarget? gender,
        string? sport,
        string? subCategory,
        decimal? minPrice,
        decimal? maxPrice,
        string? q)
    {
        ViewBag.Sports = await _productCatalogService.GetSportsAsync();
        ViewBag.SubCategories = await _productCatalogService.GetSubCategoriesAsync();

        var userId = _userManager.GetUserId(User);

        ViewBag.FavoriteIds = string.IsNullOrEmpty(userId)
            ? new List<int>()
            : await _favoritesService.GetFavoriteProductIdsAsync(userId);

        var products = await _productCatalogService.GetFilteredProductsAsync(
            gender,
            sport,
            subCategory,
            minPrice,
            maxPrice,
            q);

        return View(products);
    }

    /// <summary>
    /// Displays detailed information for a single product.
    /// </summary>
    public async Task<IActionResult> Details(int id)
    {
        var product = await _productCatalogService.GetByIdWithPhotosAsync(id);

        if (product == null)
        {
            return NotFound();
        }

        var userId = _userManager.GetUserId(User);

        ViewBag.IsFavorite = !string.IsNullOrEmpty(userId) &&
                             await _favoritesService.IsFavoriteAsync(userId, id);

        return View(product);
    }
}