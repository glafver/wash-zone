using Microsoft.EntityFrameworkCore;
using WashZone.Data;
using WashZone.Models;

namespace WashZone.Services;

public class StationService : IStationService
{
    private readonly ApplicationDbContext _context;

    public StationService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Station>> GetStationsAsync(int? packageId = null)
    {
        IQueryable<Station> query = _context.Stations;

        if (packageId.HasValue && packageId > 0)
        {
            query = query.Where(s => s.StationPackages.Any(sp => sp.PackageId == packageId));
        }

        return await query.ToListAsync();
    }

    public async Task<Station?> GetStationDetailsAsync(int id)
        => await _context.Stations
            .Include(s => s.StationPackages)
                .ThenInclude(sp => sp.Package)
                .ThenInclude(p => p.PackageFeatures)
                .ThenInclude(pf => pf.Feature)
            .FirstOrDefaultAsync(s => s.Id == id);

    public async Task<List<Package>> GetPackagesAsync()
        => await _context.Packages.ToListAsync();

    public async Task<List<Package>> GetPackagesForStationAsync(int stationId)
        => await _context.StationPackages
            .Where(sp => sp.StationId == stationId)
            .Select(sp => sp.Package)
            .ToListAsync();

    public async Task<Package?> GetPackageAsync(int id)
        => await _context.Packages.FindAsync(id);

    public Task<bool> StationExistsAsync(int id)
        => _context.Stations.AnyAsync(s => s.Id == id);

    public Task<bool> StationOffersPackageAsync(int stationId, int packageId)
        => _context.StationPackages.AnyAsync(sp => sp.StationId == stationId && sp.PackageId == packageId);
}
