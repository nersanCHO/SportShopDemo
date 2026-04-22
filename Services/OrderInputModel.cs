using System.ComponentModel.DataAnnotations;

namespace SportShop.Models;

/// <summary>
/// Input model used during checkout to collect customer delivery details.
/// Note: the file currently lives in the Services folder, but its namespace
/// is SportShop.Models in the repository. That is preserved intentionally.
/// </summary>
public class OrderInputModel
{
    /// <summary>
    /// Customer full name.
    /// </summary>
    [Required]
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// Delivery address.
    /// </summary>
    [Required]
    public string Address { get; set; } = string.Empty;

    /// <summary>
    /// Contact phone number.
    /// </summary>
    [Required]
    public string Phone { get; set; } = string.Empty;
}