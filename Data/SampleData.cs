using Microsoft.AspNetCore.Identity;
using WashZone.Models;

namespace WashZone.Data
{
    public class SampleData
    {
        public static void SeedData(ApplicationDbContext database, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            //Seed stations data to database
            if (!database.Stations.Any())
            {
                database.Stations.AddRange(new List<Station>
                    {
                        new Station { Name = "Centrum", Address = "Gustav Adolfs Torg 10, Malmö" },
                        new Station { Name = "Västra Hamnen", Address = "Dockgatan 5, Malmö" },
                        new Station { Name = "Rosengård", Address = "Rosengårds Centrum 8, Malmö" },
                        new Station { Name = "Limhamn", Address = "Limhamnsvägen 32, Malmö" },
                        new Station { Name = "Hyllie", Address = "Hyllie Boulevard 18, Malmö" },
                        new Station { Name = "Möllevången", Address = "Bergsgatan 25, Malmö" },
                        new Station { Name = "Sorgenfri", Address = "Sorgenfrivägen 15, Malmö" },
                        new Station { Name = "Kirseberg", Address = "Kirsebergstorget 4, Malmö" }
                    });
                database.SaveChanges();
            }

            //Seed packages data to database
            if (!database.Packages.Any())
            {
                database.Packages.AddRange(new List<Package>
                    {
                        new Package { Name = "Standard", Price = 150, DurationMinutes = 30 },
                        new Package { Name = "Premium", Price = 250, DurationMinutes = 60 },
                        new Package { Name = "Deluxe", Price = 400, DurationMinutes = 90 },
                        new Package { Name = "Exclusive", Price = 600, DurationMinutes = 120 },
                        new Package { Name = "Luxury", Price = 800, DurationMinutes = 180 }
                    });
                database.SaveChanges();
            }

            //Seed features data to database
            if (!database.Features.Any())
            {
                database.Features.AddRange(new List<Feature>
                    {
                        new Feature { Name = "Exterior wash" },
                        new Feature { Name = "Interior cleaning" },
                        new Feature { Name = "Waxing" },
                        new Feature { Name = "Tire shine" },
                        new Feature { Name = "Window cleaning" },
                        new Feature { Name = "Engine wash" },
                        new Feature { Name = "Interior disinfection" },
                        new Feature { Name = "Odor removal" },
                        new Feature { Name = "Ceramic coating" }
                    });
                database.SaveChanges();
            }

            //Seed package-feature data to database
            if (!database.PackageFeatures.Any())
            {
                database.PackageFeatures.AddRange(new List<PackageFeature>
                    {
                        new PackageFeature { PackageId = 1, FeatureId = 1 }, // Standard: Exterior wash
                        new PackageFeature { PackageId = 2, FeatureId = 1 }, // Premium: Exterior wash
                        new PackageFeature { PackageId = 2, FeatureId = 2 }, // + Interior cleaning
                        new PackageFeature { PackageId = 3, FeatureId = 1 }, // Deluxe: Exterior wash
                        new PackageFeature { PackageId = 3, FeatureId = 2 }, // + Interior cleaning
                        new PackageFeature { PackageId = 3, FeatureId = 3 }, // + Waxing
                        new PackageFeature { PackageId = 3, FeatureId = 4 }, // + Tire shine
                        new PackageFeature { PackageId = 4, FeatureId = 1 }, // Exclusive: Exterior wash
                        new PackageFeature { PackageId = 4, FeatureId = 2 },
                        new PackageFeature { PackageId = 4, FeatureId = 3 },
                        new PackageFeature { PackageId = 4, FeatureId = 4 },
                        new PackageFeature { PackageId = 4, FeatureId = 5 }, // + Window cleaning
                        new PackageFeature { PackageId = 4, FeatureId = 6 }, // + Engine wash
                        new PackageFeature { PackageId = 5, FeatureId = 1 }, // Luxury: All
                        new PackageFeature { PackageId = 5, FeatureId = 2 },
                        new PackageFeature { PackageId = 5, FeatureId = 3 },
                        new PackageFeature { PackageId = 5, FeatureId = 4 },
                        new PackageFeature { PackageId = 5, FeatureId = 5 },
                        new PackageFeature { PackageId = 5, FeatureId = 6 },
                        new PackageFeature { PackageId = 5, FeatureId = 7 }, // + Interior disinfection
                        new PackageFeature { PackageId = 5, FeatureId = 8 }, // + Odor removal
                        new PackageFeature { PackageId = 5, FeatureId = 9 }  // + Ceramic coating
                    });
                database.SaveChanges();
            }

            //Seed station-package data to database
            if (!database.StationPackages.Any())
            {
                database.StationPackages.AddRange(new List<StationPackage>
                    {
                        new StationPackage { StationId = 1, PackageId = 1 },
                        new StationPackage { StationId = 1, PackageId = 2 },
                        new StationPackage { StationId = 1, PackageId = 3 },

                        new StationPackage { StationId = 2, PackageId = 2 },
                        new StationPackage { StationId = 2, PackageId = 3 },
                        new StationPackage { StationId = 2, PackageId = 4 },
                        new StationPackage { StationId = 2, PackageId = 5 },

                        new StationPackage { StationId = 3, PackageId = 1 },
                        new StationPackage { StationId = 3, PackageId = 4 },
                        new StationPackage { StationId = 3, PackageId = 5 },

                        new StationPackage { StationId = 4, PackageId = 1 },
                        new StationPackage { StationId = 4, PackageId = 2 },
                        new StationPackage { StationId = 4, PackageId = 3 },
                        new StationPackage { StationId = 4, PackageId = 5 },

                        new StationPackage { StationId = 5, PackageId = 3 },
                        new StationPackage { StationId = 5, PackageId = 2 },
                        new StationPackage { StationId = 5, PackageId = 4 },
                        new StationPackage { StationId = 5, PackageId = 5 },

                        new StationPackage {StationId = 6, PackageId = 4 },
                        new StationPackage {StationId = 6, PackageId = 5 },
                        new StationPackage {StationId = 6, PackageId = 3 },

                        new StationPackage { StationId = 7, PackageId = 1 },
                        new StationPackage { StationId = 7, PackageId = 2 },
                        new StationPackage { StationId = 7, PackageId = 3 },

                        new StationPackage { StationId = 8, PackageId = 2 },
                        new StationPackage { StationId = 8, PackageId = 3 },
                        new StationPackage { StationId = 8, PackageId = 4 },
                        new StationPackage { StationId = 8, PackageId = 5 },
                    });
                database.SaveChanges();
            }

            SeedUsers(userManager, roleManager, database).Wait();
        }

        // A "naive" now (no timezone kind) for seeding booking dates. Booking times are
        // stored as `timestamp without time zone`, which requires a DateTimeKind of Unspecified.
        private static DateTime NaiveNow => DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);

        private static async Task SeedUsers(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext database)
        {
            //Seed roles data to database
            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }
            if (!await roleManager.RoleExistsAsync("User"))
            {
                await roleManager.CreateAsync(new IdentityRole("User"));
            }
            if (!await roleManager.RoleExistsAsync("StationAdmin"))
            {
                await roleManager.CreateAsync(new IdentityRole("StationAdmin"));
            }

            //Seed admin user data to database

            //When log in as admin, use:
            //email: admin@washzone.se
            //password: Admin123!

            if (await userManager.FindByEmailAsync("admin@washzone.se") == null)
            {
                var adminUser = new IdentityUser
                {
                    UserName = "admin@washzone.se",
                    Email = "admin@washzone.se",
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(adminUser, "Admin123!");
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }

            //Seed station admin user (manages station 1)
            //When log in as station admin, use:
            //email: stationadmin@washzone.se
            //password: Admin123!

            if (await userManager.FindByEmailAsync("stationadmin@washzone.se") == null)
            {
                var stationAdminUser = new IdentityUser
                {
                    UserName = "stationadmin@washzone.se",
                    Email = "stationadmin@washzone.se",
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(stationAdminUser, "Admin123!");
                await userManager.AddToRoleAsync(stationAdminUser, "StationAdmin");

                if (!database.StationAdmins.Any(sa => sa.UserId == stationAdminUser.Id))
                {
                    database.StationAdmins.Add(new StationAdmin { UserId = stationAdminUser.Id, StationId = 1 });
                    await database.SaveChangesAsync();
                }
            }

            //Seed user data to database

            var userEmails = new List<string>
                {
                    "user1@test.se",
                    "user2@test.se",
                    "user3@test.se",
                    "user4@test.se"
                };

            var userPhoneNumbers = new List<string>
                {
                    "+46701112233",
                    "+46704445566",
                    "+46707778899",
                    "+46701234567"
                };

            var userPasswords = "User123!";

            var userIds = new List<string>();

            for (int i = 0; i < userEmails.Count; i++)
            {
                var email = userEmails[i];
                var user = await userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    user = new IdentityUser
                    {
                        UserName = email,
                        Email = email,
                        EmailConfirmed = true,
                        PhoneNumber = userPhoneNumbers[i]
                    };
                    await userManager.CreateAsync(user, userPasswords);
                    await userManager.AddToRoleAsync(user, "User");
                    userIds.Add(user.Id);
                }
                else
                {
                    userIds.Add(user.Id);
                }
            }

            //Seed a dedicated demo user (pre-filled) so anyone can log in and
            //explore the app as a regular user.
            //email: demo@washzone.se
            //password: Demo123!

            var demoUser = await userManager.FindByEmailAsync("demo@washzone.se");
            if (demoUser == null)
            {
                demoUser = new IdentityUser
                {
                    UserName = "demo@washzone.se",
                    Email = "demo@washzone.se",
                    EmailConfirmed = true,
                    PhoneNumber = "+46705551234"
                };
                await userManager.CreateAsync(demoUser, "Demo123!");
                await userManager.AddToRoleAsync(demoUser, "User");
            }

            //Seed bookings data to database for different users
            if (!database.Bookings.Any())
            {
                var bookings = new List<Booking>
                    {
                        // Bookings for user1
                        new Booking { UserId = userIds[0], RegistrationNumber = "ABC123", PackageId = 1, StationId = 1, DurationMinutes = 30, Date = NaiveNow.AddDays(2) },
                        new Booking { UserId = userIds[0],RegistrationNumber = "ABC123", PackageId = 2, StationId = 3, DurationMinutes = 60, Date = NaiveNow.AddDays(5) },
                        new Booking { UserId = userIds[0], RegistrationNumber = "ABC123",PackageId = 3, StationId = 5, DurationMinutes = 90, Date = NaiveNow.AddDays(7) },

                        // Bookings for user2
                        new Booking { UserId = userIds[1],RegistrationNumber = "XYZ789", PackageId = 2, StationId = 2, DurationMinutes = 60, Date = NaiveNow.AddDays(3) },
                        new Booking { UserId = userIds[1],RegistrationNumber = "XYZ789", PackageId = 4, StationId = 4, DurationMinutes = 120, Date = NaiveNow.AddDays(6) },
                        new Booking { UserId = userIds[1],RegistrationNumber = "TESLA", PackageId = 5, StationId = 6, DurationMinutes = 180, Date = NaiveNow.AddDays(9) },

                        // Bookings for user3
                        new Booking { UserId = userIds[2],RegistrationNumber = "LMN321", PackageId = 3, StationId = 3, DurationMinutes = 90, Date = NaiveNow.AddDays(4) },
                        new Booking { UserId = userIds[2],RegistrationNumber = "LMN321", PackageId = 1, StationId = 1, DurationMinutes = 30, Date = NaiveNow.AddDays(8) },
                        new Booking { UserId = userIds[2],RegistrationNumber = "LMN321", PackageId = 5, StationId = 7, DurationMinutes = 180, Date = NaiveNow.AddDays(10) },

                        // Bookings for user4
                        new Booking { UserId = userIds[3],RegistrationNumber = "DEF456", PackageId = 4, StationId = 4, DurationMinutes = 120, Date = NaiveNow.AddDays(1) },
                        new Booking { UserId = userIds[3],RegistrationNumber = "SUPERCAR", PackageId = 2, StationId = 2, DurationMinutes = 60, Date = NaiveNow.AddDays(5) },
                        new Booking { UserId = userIds[3],RegistrationNumber = "SUPERCAR", PackageId = 3, StationId = 8, DurationMinutes = 90, Date = NaiveNow.AddDays(7) }
                    };

                database.Bookings.AddRange(bookings);
                database.SaveChanges();
            }

            //Seed saved cars (UserVehicle) so the demo accounts have pre-filled
            //registration numbers for the booking autocomplete and profile page.
            if (!database.UserVehicles.Any())
            {
                database.UserVehicles.AddRange(new List<UserVehicle>
                {
                    new UserVehicle { UserId = userIds[0], RegistrationNumber = "ABC123" },
                    new UserVehicle { UserId = userIds[1], RegistrationNumber = "XYZ789" },
                    new UserVehicle { UserId = userIds[1], RegistrationNumber = "TESLA" },
                    new UserVehicle { UserId = userIds[2], RegistrationNumber = "LMN321" },
                    new UserVehicle { UserId = userIds[3], RegistrationNumber = "DEF456" },
                    new UserVehicle { UserId = userIds[3], RegistrationNumber = "SUPERCAR" },
                });
                database.SaveChanges();
            }

            // The demo user's data is seeded separately (per-user guard) so it also
            // gets added on databases that already contain other seeded bookings/cars.
            if (!database.Bookings.Any(b => b.UserId == demoUser.Id))
            {
                database.Bookings.AddRange(new List<Booking>
                {
                    new Booking { UserId = demoUser.Id, RegistrationNumber = "ABC123", PackageId = 1, StationId = 2, DurationMinutes = 30, Date = NaiveNow.AddDays(-3) },
                    new Booking { UserId = demoUser.Id, RegistrationNumber = "ABC123", PackageId = 2, StationId = 4, DurationMinutes = 60, Date = NaiveNow.AddDays(2) },
                    new Booking { UserId = demoUser.Id, RegistrationNumber = "XYZ789", PackageId = 3, StationId = 6, DurationMinutes = 90, Date = NaiveNow.AddDays(6) },
                });
                database.SaveChanges();
            }

            if (!database.UserVehicles.Any(v => v.UserId == demoUser.Id))
            {
                database.UserVehicles.AddRange(new List<UserVehicle>
                {
                    new UserVehicle { UserId = demoUser.Id, RegistrationNumber = "ABC123" },
                    new UserVehicle { UserId = demoUser.Id, RegistrationNumber = "XYZ789" },
                });
                database.SaveChanges();
            }
        }
    }
}

