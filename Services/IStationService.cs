using Microsoft.AspNetCore.Identity;
using WashZone.Models;

namespace WashZone.Services;

public interface IStationService
{
    Task<List<Station>> GetStationsAsync(int? packageId = null);
    Task<Station?> GetStationDetailsAsync(int id);
    Task<List<Package>> GetPackagesAsync();
    Task<List<Package>> GetPackagesWithFeaturesAsync();
    Task<List<Feature>> GetFeaturesAsync();
    Task<List<Package>> GetPackagesForStationAsync(int stationId);
    Task<Package?> GetPackageAsync(int id);
    Task<bool> StationExistsAsync(int id);
    Task<bool> StationOffersPackageAsync(int stationId, int packageId);

    // Catalog management (global admin)
    Task<Station> CreateStationAsync(Station station);
    Task<ServiceResult> UpdateStationAsync(Station station);
    Task<ServiceResult> DeleteStationAsync(int id);
    Task SetStationPackagesAsync(int stationId, IEnumerable<int> packageIds);

    // Station admins
    Task<List<IdentityUser>> GetStationAdminsAsync(int stationId);
    Task<List<IdentityUser>> GetAvailableAdminUsersAsync();
    Task<int?> GetStationIdForAdminAsync(string userId);
    Task<ServiceResult> AssignStationAdminAsync(int stationId, string userId);
    Task<ServiceResult> RemoveStationAdminAsync(string userId);
}
