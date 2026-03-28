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

    [Required, StringLength(120)]
    public string Name { get; set; } = string.Empty;

    [Required, StringLength(80)]
    public string Sport { get; set; } = string.Empty;

    [Required, StringLength(80)]
    public string SubCategory { get; set; } = string.Empty;

    [Range(0.01, 100000)]
    public decimal Price { get; set; }

    public GenderTarget Gender { get; set; } = GenderTarget.Unisex;

    public ProductSizeType SizeType { get; set; } = ProductSizeType.Universal;

    [Required, StringLength(200)]
    public string AvailableSizes { get; set; } = "Universal";

    [StringLength(800)]
    public string? Description { get; set; }

    [StringLength(260)]
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
            return ClothingSizeOptions.FirstOrDefault(x => x.Equals(incoming, StringComparison.OrdinalIgnoreCase))
                   ?? string.Empty;
        }

        return ShoeSizeOptions.FirstOrDefault(x => x.Equals(incoming, StringComparison.OrdinalIgnoreCase))
               ?? string.Empty;
    }
}