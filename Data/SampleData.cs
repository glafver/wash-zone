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
                        new Package { Name = "Standard", Price = 150 },
                        new Package { Name = "Premium", Price = 250 },
                        new Package { Name = "Deluxe", Price = 400 },
                        new Package { Name = "Exklusiv", Price = 600 },
                        new Package { Name = "Lyxpaket", Price = 800 }
                    });
                database.SaveChanges();
            }

            //Seed features data to database
            if (!database.Features.Any())
            {
                database.Features.AddRange(new List<Feature>
                    {
                        new Feature { Name = "Utvändig tvätt" },
                        new Feature { Name = "Invändig rengöring" },
                        new Feature { Name = "Vaxning" },
                        new Feature { Name = "Däckglans" },
                        new Feature { Name = "Fönsterputs" },
                        new Feature { Name = "Motortvätt" },
                        new Feature { Name = "Interiör desinficering" },
                        new Feature { Name = "Luktsanering" },
                        new Feature { Name = "Keramisk beläggning" }
                    });
                database.SaveChanges();
            }

            //Seed package-feature data to database
            if (!database.PackageFeatures.Any())
            {
                database.PackageFeatures.AddRange(new List<PackageFeature>
                    {
                        new PackageFeature { PackageId = 1, FeatureId = 1 }, // Standard: Utvändig tvätt
                        new PackageFeature { PackageId = 2, FeatureId = 1 }, // Premium: Utvändig tvätt
                        new PackageFeature { PackageId = 2, FeatureId = 2 }, // + Invändig rengöring
                        new PackageFeature { PackageId = 3, FeatureId = 1 }, // Deluxe: Utvändig tvätt
                        new PackageFeature { PackageId = 3, FeatureId = 2 }, // + Invändig rengöring
                        new PackageFeature { PackageId = 3, FeatureId = 3 }, // + Vaxning
                        new PackageFeature { PackageId = 3, FeatureId = 4 }, // + Däckglans
                        new PackageFeature { PackageId = 4, FeatureId = 1 }, // Exklusiv: Utvändig tvätt
                        new PackageFeature { PackageId = 4, FeatureId = 2 },
                        new PackageFeature { PackageId = 4, FeatureId = 3 },
                        new PackageFeature { PackageId = 4, FeatureId = 4 },
                        new PackageFeature { PackageId = 4, FeatureId = 5 }, // + Fönsterputs
                        new PackageFeature { PackageId = 4, FeatureId = 6 }, // + Motortvätt
                        new PackageFeature { PackageId = 5, FeatureId = 1 }, // Lyxpaket: Allt
                        new PackageFeature { PackageId = 5, FeatureId = 2 },
                        new PackageFeature { PackageId = 5, FeatureId = 3 },
                        new PackageFeature { PackageId = 5, FeatureId = 4 },
                        new PackageFeature { PackageId = 5, FeatureId = 5 },
                        new PackageFeature { PackageId = 5, FeatureId = 6 },
                        new PackageFeature { PackageId = 5, FeatureId = 7 }, // + Interiör desinficering
                        new PackageFeature { PackageId = 5, FeatureId = 8 }, // + Luktsanering
                        new PackageFeature { PackageId = 5, FeatureId = 9 }  // + Keramisk beläggning
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

            //Seed admin user data to database

            //When log in as admin, use:
            //email: admin@washzone.se
            //password: Admin123!

            if (await userManager.FindByEmailAsync("admin@washzone.se") == null)
            {
                var adminUser = new User
                {
                    UserName = "admin@washzone.se",
                    Email = "admin@washzone.se",
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(adminUser, "Admin123!");
                await userManager.AddToRoleAsync(adminUser, "Admin");
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
                    user = new User
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

            //Seed bookings data to database for different users
            if (!database.Bookings.Any())
            {
                var bookings = new List<Booking>
                    {
                        // Bookings for user1
                        new Booking { UserId = userIds[0], RegistrationNumber = "ABC123", PackageId = 1, StationId = 1, Date = DateTime.UtcNow.AddDays(2) },
                        new Booking { UserId = userIds[0],RegistrationNumber = "ABC123", PackageId = 2, StationId = 3, Date = DateTime.UtcNow.AddDays(5) },
                        new Booking { UserId = userIds[0], RegistrationNumber = "ABC123",PackageId = 3, StationId = 5, Date = DateTime.UtcNow.AddDays(7) },

                        // Bookings for user2
                        new Booking { UserId = userIds[1],RegistrationNumber = "XYZ789", PackageId = 2, StationId = 2, Date = DateTime.UtcNow.AddDays(3) },
                        new Booking { UserId = userIds[1],RegistrationNumber = "XYZ789", PackageId = 4, StationId = 4, Date = DateTime.UtcNow.AddDays(6) },
                        new Booking { UserId = userIds[1],RegistrationNumber = "TESLA", PackageId = 5, StationId = 6, Date = DateTime.UtcNow.AddDays(9) },

                        // Bookings for user3
                        new Booking { UserId = userIds[2],RegistrationNumber = "LMN321", PackageId = 3, StationId = 3, Date = DateTime.UtcNow.AddDays(4) },
                        new Booking { UserId = userIds[2],RegistrationNumber = "LMN321", PackageId = 1, StationId = 1, Date = DateTime.UtcNow.AddDays(8) },
                        new Booking { UserId = userIds[2],RegistrationNumber = "LMN321", PackageId = 5, StationId = 7, Date = DateTime.UtcNow.AddDays(10) },

                        // Bookings for user4
                        new Booking { UserId = userIds[3],RegistrationNumber = "DEF456", PackageId = 4, StationId = 4, Date = DateTime.UtcNow.AddDays(1) },
                        new Booking { UserId = userIds[3],RegistrationNumber = "SUPERCAR", PackageId = 2, StationId = 2, Date = DateTime.UtcNow.AddDays(5) },
                        new Booking { UserId = userIds[3],RegistrationNumber = "SUPERCAR", PackageId = 3, StationId = 8, Date = DateTime.UtcNow.AddDays(7) }
                    };

                database.Bookings.AddRange(bookings);
                database.SaveChanges();
            }
        }
    }
}

