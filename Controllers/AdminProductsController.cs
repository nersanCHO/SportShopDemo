using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportShop.Models;
using SportShop.Services;

namespace SportShop.Controllers;

[Authorize(Roles = "Admin")]
public class AdminProductsController : Controller
{
    private readonly AdminProductService _adminProductService;

    public AdminProductsController(AdminProductService adminProductService)
    {
        _adminProductService = adminProductService;
    }

    public async Task<IActionResult> Index()
    {
        var products = await _adminProductService.GetAllAsync();
        return View(products);
    }

    public IActionResult Create()
    {
        return View(new Product());
    }

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

    public async Task<IActionResult> Edit(int id)
    {
        var product = await _adminProductService.GetByIdWithPhotosAsync(id);
        if (product == null)
        {
            return NotFound();
        }

        return View(product);
    }

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

    public async Task<IActionResult> Delete(int id)
    {
        var product = await _adminProductService.GetByIdAsync(id);
        if (product == null)
        {
            return NotFound();
        }

        return View(product);
    }

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