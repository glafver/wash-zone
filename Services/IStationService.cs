using WashZone.Models;

namespace WashZone.Services;

public interface IStationService
{
    Task<List<Station>> GetStationsAsync(int? packageId = null);
    Task<Station?> GetStationDetailsAsync(int id);
    Task<List<Package>> GetPackagesAsync();
    Task<List<Package>> GetPackagesForStationAsync(int stationId);
    Task<Package?> GetPackageAsync(int id);
    Task<bool> StationExistsAsync(int id);
    Task<bool> StationOffersPackageAsync(int stationId, int packageId);
}
