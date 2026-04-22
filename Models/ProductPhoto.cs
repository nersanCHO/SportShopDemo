using System.ComponentModel.DataAnnotations;

namespace SportShop.Models;

/// <summary>
/// Represents a single photo belonging to a product.
/// </summary>
public class ProductPhoto
{
    /// <summary>
    /// Primary key.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Relative path to the saved image file.
    /// </summary>
    [StringLength(260)]
    public string ImagePath { get; set; } = "/images/products/default.png";

    /// <summary>
    /// Foreign key to the owning product.
    /// </summary>
    public int ProductId { get; set; }

    /// <summary>
    /// Navigation property to the owning product.
    /// </summary>
    public Product? Product { get; set; }
}