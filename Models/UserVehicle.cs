using Microsoft.AspNetCore.Identity;

namespace WashZone.Models
{
    /// <summary>Stores a car (registration number) saved by a user for quick re-use when booking.</summary>
    public class UserVehicle
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string RegistrationNumber { get; set; } = string.Empty;

        public IdentityUser User { get; set; } = null!;
    }
}
