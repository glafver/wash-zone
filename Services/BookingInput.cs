namespace WashZone.Services;

/// <summary>
/// The data needed to create or update a booking, decoupled from the web layer.
/// </summary>
public class BookingInput
{
    public int StationId { get; set; }
    public int PackageId { get; set; }
    public string RegistrationNumber { get; set; } = string.Empty;
    public DateTime Date { get; set; }
}
