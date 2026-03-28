using System.ComponentModel.DataAnnotations;

namespace SportShop.Models;

public class CartItem
{
    public int Id { get; set; }

    [Required]
    public string UserId { get; set; } = string.Empty;

    public ApplicationUser? User { get; set; }

    public int ProductId { get; set; }

    public Product? Product { get; set; }

    [Required, StringLength(20)]
    public string SelectedSize { get; set; } = "Universal";

    [Range(1, 999)]
    public int Quantity { get; set; } = 1;
}