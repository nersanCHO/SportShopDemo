namespace SportShop.Services;

/// <summary>
/// Standard service-layer result object used to communicate success,
/// failure, not-found state, and optional messages back to controllers.
/// </summary>
public class ServiceResult
{
    /// <summary>
    /// Indicates whether the operation succeeded.
    /// </summary>
    public bool Success { get; init; }

    /// <summary>
    /// Indicates whether the requested resource was not found.
    /// </summary>
    public bool NotFound { get; init; }

    /// <summary>
    /// Optional human-readable message.
    /// </summary>
    public string? Message { get; init; }

    /// <summary>
    /// Creates a successful result.
    /// </summary>
    public static ServiceResult Ok(string? message = null)
        => new() { Success = true, Message = message };

    /// <summary>
    /// Creates a failed result.
    /// </summary>
    public static ServiceResult Fail(string message)
        => new() { Success = false, Message = message };

    /// <summary>
    /// Creates a not-found result.
    /// </summary>
    public static ServiceResult Missing(string? message = null)
        => new() { Success = false, NotFound = true, Message = message };
}