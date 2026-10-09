using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Security.Claims;
using WashZone.Data;
using WashZone.Models;

namespace WashZone.Pages
{
    [Authorize]
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
        [Range(1, int.MaxValue, ErrorMessage = "Please select a station.")]
        public int SelectedStationId { get; set; }
        [BindProperty]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a package.")]
        public int SelectedPackageId { get; set; }
        [BindProperty]
        [Required(ErrorMessage = "Please select a month.")]
        public string SelectedMonth { get; set; } = string.Empty;
        [BindProperty]
        [Range(1, 31, ErrorMessage = "Please select a valid day.")]
        public int SelectedDay { get; set; }
        [BindProperty]
        [Required(ErrorMessage = "Please select a time.")]
        public string SelectedTime { get; set; } = string.Empty;

        [BindProperty]
        [Required(ErrorMessage = "Registration number is required.")]
        [StringLength(10, MinimumLength = 2, ErrorMessage = "Registration number must be 2-10 characters.")]
        [RegularExpression("^[A-Za-z0-9]+$", ErrorMessage = "Registration number may only contain letters and digits.")]
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

            // Parse and validate the chosen date/time (also rejects dates in the past).
            if (!TryBuildBookingDate(out var finalBookingDate))
            {
                return Page();
            }

            // Ensure the station exists and actually offers the selected package.
            var stationExists = await _context.Stations.AnyAsync(s => s.Id == SelectedStationId);
            if (!stationExists)
            {
                ModelState.AddModelError(nameof(SelectedStationId), "The selected station does not exist.");
                return Page();
            }

            var packageOffered = await _context.StationPackages.AnyAsync(sp =>
                sp.StationId == SelectedStationId && sp.PackageId == SelectedPackageId);
            if (!packageOffered)
            {
                ModelState.AddModelError(nameof(SelectedPackageId), "The selected package is not offered at this station.");
                return Page();
            }

            // Prevent double-booking the same station at the same time.
            var slotTaken = await _context.Bookings.AnyAsync(b =>
                b.StationId == SelectedStationId && b.Date == finalBookingDate);
            if (slotTaken)
            {
                ModelState.AddModelError("", "This time is already booked at the selected station. Please choose another time.");
                return Page();
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            Booking = new Booking // creates the booking based on user input
            {
                UserId = userId ?? string.Empty,
                StationId = SelectedStationId,
                PackageId = SelectedPackageId,
                RegistrationNumber = RegistrationNumber.Trim().ToUpperInvariant(),
                Date = finalBookingDate
            };

            _context.Bookings.Add(Booking); //adds it to database
            await _context.SaveChangesAsync();//saves database

            TempData["SuccessMessage"] = "Your booking was successful!"; //prints success message
            return RedirectToPage("/MyBookingsPage");//return to Mybookings page
        }

        /// <summary>
        /// Builds and validates the final booking <see cref="DateTime"/> from the selected
        /// month, day and time. Rolls over to next year when the selected month has already
        /// passed this year, and rejects dates/times in the past.
        /// </summary>
        private bool TryBuildBookingDate(out DateTime bookingDate)
        {
            bookingDate = default;

            if (!DateTime.TryParseExact(SelectedMonth, "MMMM", CultureInfo.InvariantCulture, DateTimeStyles.None, out var monthDate))
            {
                ModelState.AddModelError(nameof(SelectedMonth), "Invalid month.");
                return false;
            }

            if (!TimeSpan.TryParseExact(SelectedTime, @"hh\:mm", CultureInfo.InvariantCulture, out var time))
            {
                ModelState.AddModelError(nameof(SelectedTime), "Invalid time.");
                return false;
            }

            // If the selected month has already passed this year, assume the booking is for next year.
            int year = DateTime.Now.Year;
            if (monthDate.Month < DateTime.Now.Month)
            {
                year += 1;
            }

            int maxDay = DateTime.DaysInMonth(year, monthDate.Month);
            if (SelectedDay < 1 || SelectedDay > maxDay)
            {
                ModelState.AddModelError(nameof(SelectedDay), $"Invalid day for the selected month (max {maxDay}).");
                return false;
            }

            bookingDate = new DateTime(year, monthDate.Month, SelectedDay, time.Hours, time.Minutes, 0);

            if (bookingDate < DateTime.Now)
            {
                ModelState.AddModelError("", "The selected date and time is in the past. Please choose a future time.");
                return false;
            }

            return true;
        }


    }
}

