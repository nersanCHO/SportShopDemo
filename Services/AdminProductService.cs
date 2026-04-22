using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SportShop.Data;
using SportShop.Models;

namespace SportShop.Services;

public class AdminProductService
{
    private readonly ApplicationDbContext _db;
    private readonly IWebHostEnvironment _env;

    public AdminProductService(ApplicationDbContext db, IWebHostEnvironment env)
    {
        _db = db;
        _env = env;
    }

    public async Task<List<Product>> GetAllAsync()
    {
        return await _db.Products
            .OrderBy(p => p.Name)
            .ToListAsync();
    }

    public async Task<Product?> GetByIdAsync(int id)
    {
        return await _db.Products.FindAsync(id);
    }

    public async Task<Product?> GetByIdWithPhotosAsync(int id)
    {
        return await _db.Products
            .Include(p => p.Photos)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<List<ProductPhoto>> GetPhotosAsync(int productId)
    {
        return await _db.ProductPhotos
            .Where(pp => pp.ProductId == productId)
            .ToListAsync();
    }

    public ServiceResult ValidateProduct(Product model)
    {
        model.AvailableSizes = Product.NormalizeSizeList(model.SizeType, model.AvailableSizes);

        if (model.SizeType == ProductSizeType.Universal)
        {
            model.AvailableSizes = "Universal";
            return ServiceResult.Ok();
        }

        if (string.IsNullOrWhiteSpace(model.AvailableSizes))
        {
            return ServiceResult.Fail("Избери поне един размер.");
        }

        return ServiceResult.Ok();
    }

    public async Task CreateAsync(Product model, IFormFile? titleImage, IFormFileCollection? images)
    {
        await AddImagesToNewProductAsync(model, titleImage, images);

        _db.Products.Add(model);
        await _db.SaveChangesAsync();
    }

    public async Task<bool> UpdateAsync(int id, Product model, IFormFile? titleImage, IFormFileCollection? images)
    {
        var existing = await _db.Products
            .Include(p => p.Photos)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (existing == null)
        {
            return false;
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
            foreach (var image in images.Where(i => i.Length > 0))
            {
                var saved = await SaveImageAsync(image);
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
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var product = await _db.Products
            .Include(p => p.Photos)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (product == null)
        {
            return false;
        }

        foreach (var photo in product.Photos)
        {
            TryDeleteFile(photo.ImagePath);
        }

        TryDeleteFile(product.ImagePath);

        _db.Products.Remove(product);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task DeletePhotoAsync(int photoId, int productId)
    {
        var photo = await _db.ProductPhotos
            .FirstOrDefaultAsync(pp => pp.Id == photoId && pp.ProductId == productId);

        if (photo == null)
        {
            return;
        }

        TryDeleteFile(photo.ImagePath);
        _db.ProductPhotos.Remove(photo);
        await _db.SaveChangesAsync();
    }

    private async Task AddImagesToNewProductAsync(Product model, IFormFile? titleImage, IFormFileCollection? images)
    {
        if (titleImage is { Length: > 0 })
        {
            var saved = await SaveImageAsync(titleImage);
            model.ImagePath = saved;
            model.Photos.Add(new ProductPhoto { ImagePath = saved });
        }

        if (images != null && images.Any(i => i.Length > 0))
        {
            foreach (var image in images.Where(i => i.Length > 0))
            {
                var saved = await SaveImageAsync(image);
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
    }

    private async Task<string> SaveImageAsync(IFormFile image)
    {
        var folder = Path.Combine(_env.WebRootPath, "images", "products");
        Directory.CreateDirectory(folder);

        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(image.FileName)}";
        var filePath = Path.Combine(folder, fileName);

        await using var stream = File.Create(filePath);
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
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
        catch
        {
            // Ignore deletion failures.
        }
    }
}