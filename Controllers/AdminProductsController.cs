using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportShop.Models;
using SportShop.Services;

namespace SportShop.Controllers;

/// <summary>
/// Administrative controller for managing products and product images.
/// Access is restricted to users in the Admin role.
/// </summary>
[Authorize(Roles = "Admin")]
public class AdminProductsController : Controller
{
    private readonly AdminProductService _adminProductService;

    /// <summary>
    /// Creates the controller with the required service dependency.
    /// </summary>
    public AdminProductsController(AdminProductService adminProductService)
    {
        _adminProductService = adminProductService;
    }

    /// <summary>
    /// Displays all products for administration.
    /// </summary>
    public async Task<IActionResult> Index()
    {
        var products = await _adminProductService.GetAllAsync();
        return View(products);
    }

    /// <summary>
    /// Returns the empty create form.
    /// </summary>
    public IActionResult Create()
    {
        return View(new Product());
    }

    /// <summary>
    /// Creates a new product.
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Product model, IFormFile? titleImage, IFormFileCollection? images)
    {
        var validation = _adminProductService.ValidateProduct(model);

        if (!validation.Success)
        {
            ModelState.AddModelError(nameof(Product.AvailableSizes), validation.Message!);
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        await _adminProductService.CreateAsync(model, titleImage, images);
        return RedirectToAction(nameof(Index));
    }

    /// <summary>
    /// Returns the edit form for a specific product.
    /// </summary>
    public async Task<IActionResult> Edit(int id)
    {
        var product = await _adminProductService.GetByIdWithPhotosAsync(id);

        if (product == null)
        {
            return NotFound();
        }

        return View(product);
    }

    /// <summary>
    /// Updates a specific product.
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Product model, IFormFile? titleImage, IFormFileCollection? images)
    {
        if (id != model.Id)
        {
            return BadRequest();
        }

        var validation = _adminProductService.ValidateProduct(model);

        if (!validation.Success)
        {
            ModelState.AddModelError(nameof(Product.AvailableSizes), validation.Message!);
        }

        if (!ModelState.IsValid)
        {
            model.Photos = await _adminProductService.GetPhotosAsync(model.Id);
            return View(model);
        }

        var updated = await _adminProductService.UpdateAsync(id, model, titleImage, images);

        if (!updated)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }

    /// <summary>
    /// Returns the delete confirmation page.
    /// </summary>
    public async Task<IActionResult> Delete(int id)
    {
        var product = await _adminProductService.GetByIdAsync(id);

        if (product == null)
        {
            return NotFound();
        }

        return View(product);
    }

    /// <summary>
    /// Deletes a product after confirmation.
    /// </summary>
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var deleted = await _adminProductService.DeleteAsync(id);

        if (!deleted)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }

    /// <summary>
    /// Deletes a single photo from a product gallery.
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeletePhoto(int photoId, int productId, string? returnUrl = null)
    {
        await _adminProductService.DeletePhotoAsync(photoId, productId);

        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }

        return RedirectToAction(nameof(Edit), new { id = productId });
    }
}