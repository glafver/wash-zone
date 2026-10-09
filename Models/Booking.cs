using Microsoft.AspNetCore.Identity;

namespace WashZone.Models
{
    public class Booking
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string RegistrationNumber { get; set; } = string.Empty;
        public int PackageId { get; set; }
        public int StationId { get; set; }

        /// <summary>Start time of the reserved slot.</summary>
        public DateTime Date { get; set; }

        /// <summary>Length of the reserved slot in minutes (snapshot of the package duration at booking time).</summary>
        public int DurationMinutes { get; set; }

        public IdentityUser User { get; set; } = null!;
        public Package Package { get; set; } = null!;
        public Station Station { get; set; } = null!;
    }
}

