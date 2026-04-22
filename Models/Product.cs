using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace SportShop.Models;

/// <summary>
/// Target audience for a product.
/// </summary>
public enum GenderTarget
{
    Men = 1,
    Women = 2,
    Unisex = 3
}

/// <summary>
/// Supported size strategies for products.
/// </summary>
public enum ProductSizeType
{
    Universal = 0,
    Clothing = 1,
    Shoes = 2
}

/// <summary>
/// Represents a product in the sport shop catalog.
/// </summary>
public class Product
{
    /// <summary>
    /// Predefined allowed clothing sizes.
    /// </summary>
    public static readonly string[] ClothingSizeOptions = { "XS", "S", "M", "L", "XL" };

    /// <summary>
    /// Predefined allowed shoe sizes.
    /// </summary>
    public static readonly string[] ShoeSizeOptions =
    {
        "36", "37", "38", "39", "40", "41", "42", "43", "44", "45", "46"
    };

    /// <summary>
    /// Primary key.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Product name.
    /// </summary>
    [Required(ErrorMessage = "Полето Име е задължително.")]
    [StringLength(120, ErrorMessage = "Името може да бъде най-много 120 символа.")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Sport category, such as football, tennis, or fitness.
    /// </summary>
    [Required(ErrorMessage = "Полето Спорт е задължително.")]
    [StringLength(80, ErrorMessage = "Полето Спорт може да бъде най-много 80 символа.")]
    public string Sport { get; set; } = string.Empty;

    /// <summary>
    /// More specific subcategory for the product.
    /// </summary>
    [Required(ErrorMessage = "Полето Подкатегория е задължително.")]
    [StringLength(80, ErrorMessage = "Полето Подкатегория може да бъде най-много 80 символа.")]
    public string SubCategory { get; set; } = string.Empty;

    /// <summary>
    /// Product price.
    /// </summary>
    [Range(typeof(decimal), "0.01", "100000", ErrorMessage = "Цената трябва да бъде между 0.01 и 100000 €")]
    public decimal Price { get; set; }

    /// <summary>
    /// Intended target gender.
    /// </summary>
    public GenderTarget Gender { get; set; } = GenderTarget.Unisex;

    /// <summary>
    /// Size system used by the product.
    /// </summary>
    public ProductSizeType SizeType { get; set; } = ProductSizeType.Universal;

    /// <summary>
    /// Comma-separated raw list of allowed sizes.
    /// </summary>
    [Required(ErrorMessage = "Полето Размер е задължително.")]
    [StringLength(200, ErrorMessage = "Полето Размер може да бъде най-много 200 символа.")]
    public string AvailableSizes { get; set; } = "Universal";

    /// <summary>
    /// Optional product description.
    /// </summary>
    [StringLength(800, ErrorMessage = "Описанието може да бъде най-много 800 символа.")]
    public string? Description { get; set; }

    /// <summary>
    /// Main title image path.
    /// </summary>
    [StringLength(260, ErrorMessage = "Пътят до снимката може да бъде най-много 260 символа.")]
    public string ImagePath { get; set; } = "/images/products/default.png";

    /// <summary>
    /// Additional product images.
    /// </summary>
    public List<ProductPhoto> Photos { get; set; } = new();

    /// <summary>
    /// Returns the normalized list of available sizes.
    /// </summary>
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

    /// <summary>
    /// Returns a user-friendly comma-separated size list.
    /// </summary>
    public string DisplaySizes => string.Join(", ", GetAvailableSizes());

    /// <summary>
    /// Checks whether a given size is available for the product.
    /// </summary>
    /// <param name="size">The raw or normalized size to test.</param>
    public bool HasAvailableSize(string? size)
    {
        var normalized = NormalizeSingleSize(SizeType, size);

        if (string.IsNullOrWhiteSpace(normalized))
        {
            return false;
        }

        return GetAvailableSizes().Contains(normalized, StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Normalizes a comma-separated size list according to the selected size type.
    /// </summary>
    /// <param name="sizeType">The size system used by the product.</param>
    /// <param name="rawAvailableSizes">Raw comma-separated size input.</param>
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

    /// <summary>
    /// Normalizes a single incoming size according to the selected size type.
    /// </summary>
    /// <param name="sizeType">The size system used by the product.</param>
    /// <param name="size">The raw incoming size.</param>
    public static string NormalizeSingleSize(ProductSizeType sizeType, string? size)
    {
        if (sizeType == ProductSizeType.Universal)
        {
            return "Universal";
        }

        var incoming = (size ?? string.Empty).Trim();

        if (sizeType == ProductSizeType.Clothing)
        {
            return ClothingSizeOptions
                .FirstOrDefault(x => x.Equals(incoming, StringComparison.OrdinalIgnoreCase))
                ?? string.Empty;
        }

        return ShoeSizeOptions
            .FirstOrDefault(x => x.Equals(incoming, StringComparison.OrdinalIgnoreCase))
            ?? string.Empty;
    }
}