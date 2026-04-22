using Microsoft.AspNetCore.Mvc;

namespace SportShop.Controllers;

/// <summary>
/// Provides general-purpose site actions such as the shared error page.
/// </summary>
public class HomeController : Controller
{
    /// <summary>
    /// Returns the default error view.
    /// </summary>
    public IActionResult Error() => View();
}