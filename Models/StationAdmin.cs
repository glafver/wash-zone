using Microsoft.AspNetCore.Identity;

namespace WashZone.Models
{
    /// <summary>
    /// Links a user to the station they manage as a "station admin".
    /// A station admin only sees their own station and its bookings.
    /// </summary>
    public class StationAdmin
    {
        public string UserId { get; set; } = string.Empty;
        public IdentityUser User { get; set; } = null!;

        public int StationId { get; set; }
        public Station Station { get; set; } = null!;
    }
}
