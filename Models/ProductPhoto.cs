using System.ComponentModel.DataAnnotations;

namespace SportShop.Models;

public class ProductPhoto
{
    public int Id { get; set; }

    [StringLength(260)]
    public string ImagePath { get; set; } = "/images/products/default.png";

    // FK
    public int ProductId { get; set; }
    public Product? Product { get; set; }
}