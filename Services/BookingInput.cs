namespace WashZone.Services;

/// <summary>
/// The data needed to create or update a booking, decoupled from the web layer.
/// </summary>
public class BookingInput
{
    public int StationId { get; set; }
    public int PackageId { get; set; }
    public string RegistrationNumber { get; set; } = string.Empty;

    /// <summary>Start time of the reserved slot.</summary>
    public DateTime Date { get; set; }

    /// <summary>Length of the reserved slot in minutes (from the package).</summary>
    public int DurationMinutes { get; set; }
}
