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
        => View(await _db.Products.OrderBy(p => p.Name).ToListAsync());

    public IActionResult Create() => View(new Product());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Product model, IFormFile? titleImage, IFormFileCollection? images)
    {
        if (!ModelState.IsValid) return View(model);

        // If admin provided a separate title image, save it and set legacy ImagePath.
        if (titleImage is { Length: > 0 })
        {
            var saved = await SaveImageAsync(titleImage);
            model.ImagePath = saved;
            // also add it to Photos so it appears in carousel
            model.Photos.Add(new ProductPhoto { ImagePath = saved });
        }

        // Save uploaded additional images
        if (images != null && images.Any(i => i.Length > 0))
        {
            foreach (var img in images.Where(i => i.Length > 0))
            {
                var saved = await SaveImageAsync(img);
                model.Photos.Add(new ProductPhoto { ImagePath = saved });
            }

            // if no separate titleImage uploaded, use first uploaded additional image as legacy ImagePath
            if (string.IsNullOrEmpty(model.ImagePath))
            {
                var first = model.Photos.FirstOrDefault();
                if (first != null)
                    model.ImagePath = first.ImagePath;
            }
        }

        _db.Products.Add(model);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {

        var p = await _db.Products.Include(p => p.Photos).FirstOrDefaultAsync(p => p.Id == id);
        if (p == null) return NotFound();
        return View(p);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Product model, IFormFile? titleImage, IFormFileCollection? images)
    {
        if (id != model.Id) return BadRequest();
        if (!ModelState.IsValid) return View(model);

        var existing = await _db.Products.Include(p => p.Photos).FirstOrDefaultAsync(p => p.Id == id);
        if (existing == null) return NotFound();
        existing.Size = model.Size;
        existing.Name = model.Name;
        existing.Sport = model.Sport;
        existing.SubCategory = model.SubCategory;
        existing.Price = model.Price;
        existing.Gender = model.Gender;
        existing.Description = model.Description;

        // Replace title image if provided
        if (titleImage is { Length: > 0 })
        {
            // delete previous title image file if stored in images/products
            TryDeleteFile(existing.ImagePath);

            var saved = await SaveImageAsync(titleImage);
            existing.ImagePath = saved;

            // add the new title image to photos as well (avoid duplicates if same image uploaded twice)
            if (!existing.Photos.Any(pp => pp.ImagePath == saved))
                existing.Photos.Add(new ProductPhoto { ImagePath = saved });
        }

        // Add additional uploaded images
        if (images != null && images.Any(i => i.Length > 0))
        {
            foreach (var img in images.Where(i => i.Length > 0))
            {
                var saved = await SaveImageAsync(img);
                existing.Photos.Add(new ProductPhoto { ImagePath = saved });
            }

            // ensure legacy ImagePath points to first photo if not set
            var first = existing.Photos.FirstOrDefault();
            if (first != null && string.IsNullOrEmpty(existing.ImagePath))
                existing.ImagePath = first.ImagePath;
        }

        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var p = await _db.Products.FindAsync(id);
        if (p == null) return NotFound();
        return View(p);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var p = await _db.Products.Include(p => p.Photos).FirstOrDefaultAsync(p => p.Id == id);
        if (p == null) return NotFound();

        // delete files from disk for associated photos and main image
        foreach (var ph in p.Photos)
        {
            TryDeleteFile(ph.ImagePath);
        }
        TryDeleteFile(p.ImagePath);

        _db.Products.Remove(p);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    // delete a product photo
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeletePhoto(int photoId, int productId, string? returnUrl = null)
    {
        var photo = await _db.ProductPhotos.FirstOrDefaultAsync(pp => pp.Id == photoId && pp.ProductId == productId);
        if (photo != null)
        {
            TryDeleteFile(photo.ImagePath);
            _db.ProductPhotos.Remove(photo);
            await _db.SaveChangesAsync();
        }

        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            return Redirect(returnUrl);

        return RedirectToAction(nameof(Edit), new { id = productId });
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
        if (string.IsNullOrEmpty(imagePath)) return;
        if (!imagePath.StartsWith("/images/products/")) return;

        var fileName = imagePath.Replace("/images/products/", "");
        var filePath = Path.Combine(_env.WebRootPath, "images", "products", fileName);
        try
        {
            if (System.IO.File.Exists(filePath))
                System.IO.File.Delete(filePath);
        }
        catch
        {
            // ignore deletion failures
        }
    }
}
