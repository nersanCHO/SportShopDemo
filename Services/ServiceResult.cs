namespace SportShop.Services;

public class ServiceResult
{
    public bool Success { get; init; }
    public bool NotFound { get; init; }
    public string? Message { get; init; }

    public static ServiceResult Ok(string? message = null)
        => new() { Success = true, Message = message };

    public static ServiceResult Fail(string message)
        => new() { Success = false, Message = message };

    public static ServiceResult Missing(string? message = null)
        => new() { Success = false, NotFound = true, Message = message };
}