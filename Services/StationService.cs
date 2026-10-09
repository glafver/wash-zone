using Microsoft.AspNetCore.Identity;
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

        return await query.OrderBy(s => s.Name).ToListAsync();
    }

    public async Task<Station?> GetStationDetailsAsync(int id)
        => await _context.Stations
            .Include(s => s.StationPackages)
                .ThenInclude(sp => sp.Package)
                .ThenInclude(p => p.PackageFeatures)
                .ThenInclude(pf => pf.Feature)
            .FirstOrDefaultAsync(s => s.Id == id);

    public async Task<List<Package>> GetPackagesAsync()
        => await _context.Packages.OrderBy(p => p.Name).ToListAsync();

    public async Task<List<Package>> GetPackagesWithFeaturesAsync()
        => await _context.Packages
            .Include(p => p.PackageFeatures)
                .ThenInclude(pf => pf.Feature)
            .OrderBy(p => p.Price)
            .ToListAsync();

    public async Task<List<Feature>> GetFeaturesAsync()
        => await _context.Features.OrderBy(f => f.Name).ToListAsync();

    public async Task<List<Package>> GetPackagesForStationAsync(int stationId)
        => await _context.StationPackages
            .Where(sp => sp.StationId == stationId)
            .Select(sp => sp.Package)
            .OrderBy(p => p.Name)
            .ToListAsync();

    public async Task<Package?> GetPackageAsync(int id)
        => await _context.Packages.FindAsync(id);

    public Task<bool> StationExistsAsync(int id)
        => _context.Stations.AnyAsync(s => s.Id == id);

    public Task<bool> StationOffersPackageAsync(int stationId, int packageId)
        => _context.StationPackages.AnyAsync(sp => sp.StationId == stationId && sp.PackageId == packageId);

    public async Task<Station> CreateStationAsync(Station station)
    {
        _context.Stations.Add(station);
        await _context.SaveChangesAsync();
        return station;
    }

    public async Task<ServiceResult> UpdateStationAsync(Station station)
    {
        var existing = await _context.Stations.FindAsync(station.Id);
        if (existing == null) return ServiceResult.NotFound("Station not found.");

        existing.Name = station.Name;
        existing.Address = station.Address;
        await _context.SaveChangesAsync();
        return ServiceResult.Success();
    }

    public async Task<ServiceResult> DeleteStationAsync(int id)
    {
        var station = await _context.Stations.FindAsync(id);
        if (station == null) return ServiceResult.NotFound("Station not found.");

        var hasBookings = await _context.Bookings.AnyAsync(b => b.StationId == id);
        if (hasBookings) return ServiceResult.Conflict("Cannot delete a station that has bookings.");

        _context.Stations.Remove(station);
        await _context.SaveChangesAsync();
        return ServiceResult.Success();
    }

    public async Task SetStationPackagesAsync(int stationId, IEnumerable<int> packageIds)
    {
        var existing = await _context.StationPackages.Where(sp => sp.StationId == stationId).ToListAsync();
        _context.StationPackages.RemoveRange(existing);

        foreach (var packageId in packageIds.Distinct())
        {
            _context.StationPackages.Add(new StationPackage { StationId = stationId, PackageId = packageId });
        }

        await _context.SaveChangesAsync();
    }

    public async Task<List<IdentityUser>> GetStationAdminsAsync(int stationId)
        => await _context.StationAdmins
            .Where(sa => sa.StationId == stationId)
            .Select(sa => sa.User)
            .ToListAsync();

    public async Task<List<IdentityUser>> GetAvailableAdminUsersAsync()
        => await _context.Users.OrderBy(u => u.UserName).ToListAsync();

    public async Task<int?> GetStationIdForAdminAsync(string userId)
        => await _context.StationAdmins
            .Where(sa => sa.UserId == userId)
            .Select(sa => (int?)sa.StationId)
            .FirstOrDefaultAsync();

    public async Task<ServiceResult> AssignStationAdminAsync(int stationId, string userId)
    {
        if (!await _context.Users.AnyAsync(u => u.Id == userId))
        {
            return ServiceResult.NotFound("User not found.");
        }

        // A user can only manage a single station, so replace any existing assignment.
        var existing = await _context.StationAdmins.FirstOrDefaultAsync(sa => sa.UserId == userId);
        if (existing != null)
        {
            existing.StationId = stationId;
        }
        else
        {
            _context.StationAdmins.Add(new StationAdmin { UserId = userId, StationId = stationId });
        }

        await _context.SaveChangesAsync();
        return ServiceResult.Success();
    }

    public async Task<ServiceResult> RemoveStationAdminAsync(string userId)
    {
        var existing = await _context.StationAdmins.FirstOrDefaultAsync(sa => sa.UserId == userId);
        if (existing == null) return ServiceResult.NotFound("Station admin assignment not found.");

        _context.StationAdmins.Remove(existing);
        await _context.SaveChangesAsync();
        return ServiceResult.Success();
    }
}
