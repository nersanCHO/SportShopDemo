using System.ComponentModel.DataAnnotations;

namespace SportShop.Models;

public class Favorite
{
    public int Id { get; set; }

    [Required]
    public string UserId { get; set; } = string.Empty;

    public int ProductId { get; set; }

    public ApplicationUser? User { get; set; }
    public Product? Product { get; set; }
}
