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
        public DateTime Date { get; set; }

        public IdentityUser User { get; set; } = null!;
        public Package Package { get; set; } = null!;
        public Station Station { get; set; } = null!;
    }
}

