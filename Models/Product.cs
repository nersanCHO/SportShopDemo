using System.ComponentModel.DataAnnotations;

namespace SportShop.Models;

public enum GenderTarget { Men = 1, Women = 2, Unisex = 3 }

public class Product
{
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

    [StringLength(800)]
    public string? Description { get; set; }

    [StringLength(260)]
    public string ImagePath { get; set; } = "/images/products/default.png";

    // New: multiple photos for a product
    public List<ProductPhoto> Photos { get; set; } = new();
}
