using System.ComponentModel.DataAnnotations;

namespace SportShop.Models;

/// <summary>
/// Represents a product selected by a user and stored in their shopping cart.
/// </summary>
public class CartItem
{
    /// <summary>
    /// Primary key.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// ID of the user who owns this cart item.
    /// </summary>
    [Required]
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// Navigation property to the owning user.
    /// </summary>
    public ApplicationUser? User { get; set; }

    /// <summary>
    /// ID of the selected product.
    /// </summary>
    public int ProductId { get; set; }

    /// <summary>
    /// Navigation property to the selected product.
    /// </summary>
    public Product? Product { get; set; }

    /// <summary>
    /// Selected size for the product.
    /// </summary>
    [Required, StringLength(20)]
    public string SelectedSize { get; set; } = "Universal";

    /// <summary>
    /// Quantity of the selected item.
    /// </summary>
    [Range(1, 999)]
    public int Quantity { get; set; } = 1;
}