namespace WashZone.Models
{
    public class Package
    {
        public int Id { get; set; }
        public int Price { get; set; }
        public string Name { get; set; } = string.Empty;
  
       
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
        public ICollection<PackageFeature> PackageFeatures { get; set; } = new List<PackageFeature>();
        public ICollection<StationPackage> StationPackages { get; set; } = new List<StationPackage>();
    }
}

