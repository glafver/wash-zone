namespace WashZone.Models
{
    public class Station
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
        public ICollection<StationPackage> StationPackages { get; set; } = new List<StationPackage>();

    }
}

