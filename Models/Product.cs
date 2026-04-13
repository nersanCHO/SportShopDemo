using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace SportShop.Models;

public enum GenderTarget
{
    Men = 1,
    Women = 2,
    Unisex = 3
}

public enum ProductSizeType
{
    Universal = 0,
    Clothing = 1,
    Shoes = 2
}

public class Product
{
    public static readonly string[] ClothingSizeOptions = { "XS", "S", "M", "L", "XL" };

    public static readonly string[] ShoeSizeOptions = { "36", "37", "38", "39", "40", "41", "42", "43", "44", "45", "46" };

    public int Id { get; set; }

    [Required(ErrorMessage = "Полето Име е задължително.")]
    [StringLength(120, ErrorMessage = "Името може да бъде най-много 120 символа.")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Полето Спорт е задължително.")]
    [StringLength(80, ErrorMessage = "Полето Спорт може да бъде най-много 80 символа.")]
    public string Sport { get; set; } = string.Empty;

    [Required(ErrorMessage = "Полето Подкатегория е задължително.")]
    [StringLength(80, ErrorMessage = "Полето Подкатегория може да бъде най-много 80 символа.")]
    public string SubCategory { get; set; } = string.Empty;

    [Range(typeof(decimal), "0.01", "100000", ErrorMessage = "Цената трябва да бъде между 0.01 и 100000 лв.")]
    public decimal Price { get; set; }

    public GenderTarget Gender { get; set; } = GenderTarget.Unisex;

    public ProductSizeType SizeType { get; set; } = ProductSizeType.Universal;

    [Required(ErrorMessage = "Полето Размер е задължително.")]
    [StringLength(200, ErrorMessage = "Полето Размер може да бъде най-много 200 символа.")]
    public string AvailableSizes { get; set; } = "Universal";

    [StringLength(800, ErrorMessage = "Описанието може да бъде най-много 800 символа.")]
    public string? Description { get; set; }

    [StringLength(260, ErrorMessage = "Пътят до снимката може да бъде най-много 260 символа.")]
    public string ImagePath { get; set; } = "/images/products/default.png";

    public List<ProductPhoto> Photos { get; set; } = new();

    public IReadOnlyList<string> GetAvailableSizes()
    {
        var sizes = (AvailableSizes ?? string.Empty)
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (sizes.Count == 0)
        {
            return new[] { "Universal" };
        }

        return sizes;
    }

    public string DisplaySizes => string.Join(", ", GetAvailableSizes());

    public bool HasAvailableSize(string? size)
    {
        var normalized = NormalizeSingleSize(SizeType, size);

        if (string.IsNullOrWhiteSpace(normalized))
        {
            return false;
        }

        return GetAvailableSizes().Contains(normalized, StringComparer.OrdinalIgnoreCase);
    }

    public static string NormalizeSizeList(ProductSizeType sizeType, string? rawAvailableSizes)
    {
        if (sizeType == ProductSizeType.Universal)
        {
            return "Universal";
        }

        var incoming = (rawAvailableSizes ?? string.Empty)
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .ToList();

        if (sizeType == ProductSizeType.Clothing)
        {
            var normalized = ClothingSizeOptions
                .Where(x => incoming.Contains(x, StringComparer.OrdinalIgnoreCase))
                .ToArray();

            return string.Join(",", normalized);
        }

        var shoes = ShoeSizeOptions
            .Where(x => incoming.Contains(x, StringComparer.OrdinalIgnoreCase))
            .ToArray();

        return string.Join(",", shoes);
    }

    public static string NormalizeSingleSize(ProductSizeType sizeType, string? size)
    {
        if (sizeType == ProductSizeType.Universal)
        {
            return "Universal";
        }

        var incoming = (size ?? string.Empty).Trim();

        if (sizeType == ProductSizeType.Clothing)
        {
            return ClothingSizeOptions.FirstOrDefault(x => x.Equals(incoming, StringComparison.OrdinalIgnoreCase)) ?? string.Empty;
        }

        return ShoeSizeOptions.FirstOrDefault(x => x.Equals(incoming, StringComparison.OrdinalIgnoreCase)) ?? string.Empty;
    }
}