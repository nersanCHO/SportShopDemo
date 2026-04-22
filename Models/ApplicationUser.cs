using Microsoft.AspNetCore.Identity;

namespace SportShop.Models;

/// <summary>
/// Application-specific user entity.
/// Currently it extends the built-in IdentityUser without adding custom properties,
/// but it exists so custom user fields can be added later if needed.
/// </summary>
public class ApplicationUser : IdentityUser
{
}