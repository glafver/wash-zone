using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Security.Claims;
using WashZone.Data;
using WashZone.Models;

namespace WashZone.Pages
{
    public class BookPageModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _UserManager;

        private readonly ILogger<BookPageModel> _logger;

        public BookPageModel(ApplicationDbContext context, ILogger<BookPageModel> logger, UserManager<IdentityUser> UserManager)
        {
            _context = context;
            _UserManager = UserManager;
            _logger = logger;
        }


        [BindProperty]
        public int SelectedStationId { get; set; }
        [BindProperty]
        public int SelectedPackageId { get; set; }
        [BindProperty]
        public string SelectedMonth { get; set; } = string.Empty;
        [BindProperty]
        public int SelectedDay { get; set; }
        [BindProperty]
        public string SelectedTime { get; set; } = string.Empty;

        [BindProperty]
        public string RegistrationNumber { get; set; } = string.Empty;

        public List<Station> Stations { get; set; } = new List<Station>();
        public List<Package> Packages { get; set; } = new List<Package>();

        public Booking Booking { get; set; } = new Booking();


        public async Task<IActionResult> OnGetPackagesAsync(int stationId)
        {
            var station = await _context.Stations
                .Include(s => s.StationPackages)
                .ThenInclude(sp => sp.Package)
                .FirstOrDefaultAsync(s => s.Id == stationId);

            if (station == null)
            {
                return NotFound();
            }

            // Extract available packages
            var packages = station.StationPackages.Select(sp => new
            {
                sp.Package.Id,
                sp.Package.Name
            }).ToList();

            return new JsonResult(packages);
        }

        public async Task OnGetAsync()
        {

            Stations = await _context.Stations.ToListAsync();
            Packages = await _context.Packages.ToListAsync();


            if (Stations == null || !Stations.Any()) //if fail to get station and package data
            {
                ModelState.AddModelError("", "No stations found. Please check your database.");
            }

            if (Packages == null || !Packages.Any())
            {
                ModelState.AddModelError("", "No packages found. Please check your database.");
            }

        }


        public async Task<IActionResult> OnPostAsync()
        {
            _logger.LogInformation("OnPostAsync() started...");

            if (!ModelState.IsValid)
            {

                return Page();
            }
            try
            {
                //creates a variable for the time and date the user has chosen
                int monthNumber = DateTime.ParseExact(SelectedMonth, "MMMM", System.Globalization.CultureInfo.InvariantCulture).Month;
                DateTime selectedTimeParsed = DateTime.ParseExact(SelectedTime, "HH:mm", System.Globalization.CultureInfo.InvariantCulture);
                DateTime finalBookingDate = new DateTime(DateTime.Now.Year, monthNumber, SelectedDay, selectedTimeParsed.Hour, selectedTimeParsed.Minute, 0);

                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                Booking = new Booking // creates the booking based on user input
                {
                    UserId = userId ?? string.Empty,
                    StationId = SelectedStationId,
                    PackageId = SelectedPackageId,
                    RegistrationNumber = RegistrationNumber,
                    Date = finalBookingDate
                };

                _context.Bookings.Add(Booking); //adds it to database
                await _context.SaveChangesAsync();//saves database

                TempData["SuccessMessage"] = "Your booking was successful!"; //prints success message
                return RedirectToPage("/MyBookingsPage");//return to Mybookings page
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                ModelState.AddModelError("", "Invalid date selection. Please try again.");
                return Page();
            }

        }


    }
}

