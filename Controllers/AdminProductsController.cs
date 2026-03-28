using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SportShop.Data;
using SportShop.Models;

namespace SportShop.Controllers;

[Authorize(Roles = "Admin")]
public class AdminProductsController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly IWebHostEnvironment _env;

    public AdminProductsController(ApplicationDbContext db, IWebHostEnvironment env)
    {
        _db = db;
        _env = env;
    }

    public async Task<IActionResult> Index()
    {
        var products = await _db.Products
            .OrderBy(p => p.Name)
            .ToListAsync();

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
        NormalizeAndValidateSizes(model);

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        if (titleImage is { Length: > 0 })
        {
            var saved = await SaveImageAsync(titleImage);
            model.ImagePath = saved;
            model.Photos.Add(new ProductPhoto { ImagePath = saved });
        }

        if (images != null && images.Any(i => i.Length > 0))
        {
            foreach (var img in images.Where(i => i.Length > 0))
            {
                var saved = await SaveImageAsync(img);
                model.Photos.Add(new ProductPhoto { ImagePath = saved });
            }

            if (string.IsNullOrWhiteSpace(model.ImagePath))
            {
                var first = model.Photos.FirstOrDefault();
                if (first != null)
                {
                    model.ImagePath = first.ImagePath;
                }
            }
        }

        _db.Products.Add(model);
        await _db.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var product = await _db.Products
            .Include(p => p.Photos)
            .FirstOrDefaultAsync(p => p.Id == id);

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

        NormalizeAndValidateSizes(model);

        if (!ModelState.IsValid)
        {
            model.Photos = await _db.ProductPhotos
                .Where(pp => pp.ProductId == model.Id)
                .ToListAsync();

            return View(model);
        }

        var existing = await _db.Products
            .Include(p => p.Photos)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (existing == null)
        {
            return NotFound();
        }

        existing.Name = model.Name;
        existing.Sport = model.Sport;
        existing.SubCategory = model.SubCategory;
        existing.Price = model.Price;
        existing.Gender = model.Gender;
        existing.SizeType = model.SizeType;
        existing.AvailableSizes = model.AvailableSizes;
        existing.Description = model.Description;

        if (titleImage is { Length: > 0 })
        {
            TryDeleteFile(existing.ImagePath);

            var saved = await SaveImageAsync(titleImage);
            existing.ImagePath = saved;

            if (!existing.Photos.Any(pp => pp.ImagePath == saved))
            {
                existing.Photos.Add(new ProductPhoto { ImagePath = saved });
            }
        }

        if (images != null && images.Any(i => i.Length > 0))
        {
            foreach (var img in images.Where(i => i.Length > 0))
            {
                var saved = await SaveImageAsync(img);
                existing.Photos.Add(new ProductPhoto { ImagePath = saved });
            }

            if (string.IsNullOrWhiteSpace(existing.ImagePath))
            {
                var first = existing.Photos.FirstOrDefault();
                if (first != null)
                {
                    existing.ImagePath = first.ImagePath;
                }
            }
        }

        await _db.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var product = await _db.Products.FindAsync(id);

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
        var product = await _db.Products
            .Include(p => p.Photos)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (product == null)
        {
            return NotFound();
        }

        foreach (var photo in product.Photos)
        {
            TryDeleteFile(photo.ImagePath);
        }

        TryDeleteFile(product.ImagePath);

        _db.Products.Remove(product);
        await _db.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeletePhoto(int photoId, int productId, string? returnUrl = null)
    {
        var photo = await _db.ProductPhotos
            .FirstOrDefaultAsync(pp => pp.Id == photoId && pp.ProductId == productId);

        if (photo != null)
        {
            TryDeleteFile(photo.ImagePath);
            _db.ProductPhotos.Remove(photo);
            await _db.SaveChangesAsync();
        }

        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }

        return RedirectToAction(nameof(Edit), new { id = productId });
    }

    private void NormalizeAndValidateSizes(Product model)
    {
        model.AvailableSizes = Product.NormalizeSizeList(model.SizeType, model.AvailableSizes);

        if (model.SizeType == ProductSizeType.Universal)
        {
            model.AvailableSizes = "Universal";
            return;
        }

        if (string.IsNullOrWhiteSpace(model.AvailableSizes))
        {
            ModelState.AddModelError(nameof(Product.AvailableSizes), "Избери поне един размер.");
        }
    }

    private async Task<string> SaveImageAsync(IFormFile image)
    {
        var folder = Path.Combine(_env.WebRootPath, "images", "products");
        Directory.CreateDirectory(folder);

        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(image.FileName)}";
        var filePath = Path.Combine(folder, fileName);

        await using var stream = System.IO.File.Create(filePath);
        await image.CopyToAsync(stream);

        return $"/images/products/{fileName}";
    }

    private void TryDeleteFile(string? imagePath)
    {
        if (string.IsNullOrWhiteSpace(imagePath))
        {
            return;
        }

        if (!imagePath.StartsWith("/images/products/", StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        var fileName = imagePath.Replace("/images/products/", string.Empty, StringComparison.OrdinalIgnoreCase);
        var filePath = Path.Combine(_env.WebRootPath, "images", "products", fileName);

        try
        {
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
        }
        catch
        {
            // ignore deletion failures
        }
    }
}