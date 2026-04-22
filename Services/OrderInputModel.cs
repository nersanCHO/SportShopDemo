using System.ComponentModel.DataAnnotations;

namespace SportShop.Models;

public class OrderInputModel
{
    [Required]
    public string FullName { get; set; } = string.Empty;

    [Required]
    public string Address { get; set; } = string.Empty;

    [Required]
    public string Phone { get; set; } = string.Empty;
}