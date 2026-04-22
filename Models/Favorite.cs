using System.ComponentModel.DataAnnotations;

namespace SportShop.Models;

/// <summary>
/// Represents a user-to-product favorite relationship.
/// </summary>
public class Favorite
{
    /// <summary>
    /// Primary key.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// ID of the user who marked the product as favorite.
    /// </summary>
    [Required]
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// ID of the favorited product.
    /// </summary>
    public int ProductId { get; set; }

    /// <summary>
    /// Navigation property to the owning user.
    /// </summary>
    public ApplicationUser? User { get; set; }

    /// <summary>
    /// Navigation property to the favorited product.
    /// </summary>
    public Product? Product { get; set; }
}