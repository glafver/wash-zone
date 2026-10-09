namespace WashZone.Services;

public enum ServiceErrorType
{
    None,
    NotFound,
    Forbidden,
    Conflict,
}

/// <summary>
/// A lightweight result returned by service operations that can fail for
/// business reasons (e.g. a slot is already booked).
/// </summary>
public class ServiceResult
{
    public bool Succeeded { get; private set; }
    public string? Error { get; private set; }
    public ServiceErrorType ErrorType { get; private set; }

    public static ServiceResult Success() => new() { Succeeded = true };

    public static ServiceResult NotFound(string error = "The requested resource was not found.")
        => new() { Error = error, ErrorType = ServiceErrorType.NotFound };

    public static ServiceResult Forbidden(string error = "You are not authorized to perform this action.")
        => new() { Error = error, ErrorType = ServiceErrorType.Forbidden };

    public static ServiceResult Conflict(string error)
        => new() { Error = error, ErrorType = ServiceErrorType.Conflict };
}
