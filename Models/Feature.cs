namespace WashZone.Models
{
    public class Feature
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public ICollection<PackageFeature> PackageFeatures { get; set; } = new List<PackageFeature>();
    }
}


