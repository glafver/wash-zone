using Microsoft.AspNetCore.Identity;

namespace WashZone.Models
{
    public class User : IdentityUser
    {
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}

