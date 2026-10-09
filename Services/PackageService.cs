using Microsoft.EntityFrameworkCore;
using WashZone.Data;
using WashZone.Models;

namespace WashZone.Services;

public class PackageService : IPackageService
{
    private readonly ApplicationDbContext _context;

    public PackageService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Package>> GetPackagesAsync()
        => await _context.Packages.OrderBy(p => p.Name).ToListAsync();

    public async Task<Package?> GetPackageWithFeaturesAsync(int id)
        => await _context.Packages
            .Include(p => p.PackageFeatures)
            .FirstOrDefaultAsync(p => p.Id == id);

    public async Task<List<Feature>> GetFeaturesAsync()
        => await _context.Features.OrderBy(f => f.Name).ToListAsync();

    public async Task<Package> CreatePackageAsync(Package package)
    {
        _context.Packages.Add(package);
        await _context.SaveChangesAsync();
        return package;
    }

    public async Task<ServiceResult> UpdatePackageAsync(Package package)
    {
        var existing = await _context.Packages.FindAsync(package.Id);
        if (existing == null) return ServiceResult.NotFound("Package not found.");

        existing.Name = package.Name;
        existing.Price = package.Price;
        existing.DurationMinutes = package.DurationMinutes;
        await _context.SaveChangesAsync();
        return ServiceResult.Success();
    }

    public async Task<ServiceResult> DeletePackageAsync(int id)
    {
        var package = await _context.Packages.FindAsync(id);
        if (package == null) return ServiceResult.NotFound("Package not found.");

        var hasBookings = await _context.Bookings.AnyAsync(b => b.PackageId == id);
        if (hasBookings) return ServiceResult.Conflict("Cannot delete a package that has bookings.");

        _context.Packages.Remove(package);
        await _context.SaveChangesAsync();
        return ServiceResult.Success();
    }

    public async Task SetPackageFeaturesAsync(int packageId, IEnumerable<int> featureIds)
    {
        var existing = await _context.PackageFeatures.Where(pf => pf.PackageId == packageId).ToListAsync();
        _context.PackageFeatures.RemoveRange(existing);

        foreach (var featureId in featureIds.Distinct())
        {
            _context.PackageFeatures.Add(new PackageFeature { PackageId = packageId, FeatureId = featureId });
        }

        await _context.SaveChangesAsync();
    }
}
