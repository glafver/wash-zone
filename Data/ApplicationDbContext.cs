using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WashZone.Models;

namespace WashZone.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Station> Stations { get; set; }
        public DbSet<Package> Packages { get; set; }
        public DbSet<Feature> Features { get; set; }
        public DbSet<PackageFeature> PackageFeatures { get; set; }

        public DbSet<StationPackage> StationPackages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Define many-to-many relationship between Package and Feature

            modelBuilder.Entity<PackageFeature>()
                .HasKey(pf => new { pf.PackageId, pf.FeatureId });

            modelBuilder.Entity<PackageFeature>()
                .HasOne(pf => pf.Package)
                .WithMany(p => p.PackageFeatures)
                .HasForeignKey(pf => pf.PackageId);

            modelBuilder.Entity<PackageFeature>()
                .HasOne(pf => pf.Feature)
                .WithMany(f => f.PackageFeatures)
                .HasForeignKey(pf => pf.FeatureId);

            // Define many-to-many relationship between Station and Package
            modelBuilder.Entity<StationPackage>()
                .HasKey(sp => new { sp.StationId, sp.PackageId });

            modelBuilder.Entity<StationPackage>()
                .HasOne(sp => sp.Station)
                .WithMany(s => s.StationPackages)
                .HasForeignKey(sp => sp.StationId);

            modelBuilder.Entity<StationPackage>()
                .HasOne(sp => sp.Package)
                .WithMany(p => p.StationPackages)
                .HasForeignKey(sp => sp.PackageId);
        }
    }
}

