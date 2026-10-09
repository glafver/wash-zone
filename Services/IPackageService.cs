using WashZone.Models;

namespace WashZone.Services;

public interface IPackageService
{
    Task<List<Package>> GetPackagesAsync();
    Task<List<Package>> GetPackagesWithFeaturesAsync();
    Task<Package?> GetPackageWithFeaturesAsync(int id);
    Task<List<Feature>> GetFeaturesAsync();
    Task<Package> CreatePackageAsync(Package package);
    Task<ServiceResult> UpdatePackageAsync(Package package);
    Task<ServiceResult> DeletePackageAsync(int id);
    Task SetPackageFeaturesAsync(int packageId, IEnumerable<int> featureIds);
}
