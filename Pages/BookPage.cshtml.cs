using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Security.Claims;
using WashZone.Data;
using WashZone.Models;
using WashZone.Services;

namespace WashZone.Pages
{
    [Authorize]
    public class BookPageModel : PageModel
    {
        private const int CalendarDays = 7;

        private readonly IStationService _stationService;
        private readonly IBookingService _bookingService;
        private readonly ILogger<BookPageModel> _logger;
        private readonly ApplicationDbContext _context;

        public BookPageModel(IStationService stationService, IBookingService bookingService, ILogger<BookPageModel> logger, ApplicationDbContext context)
        {
            _stationService = stationService;
            _bookingService = bookingService;
            _logger = logger;
            _context = context;
        }

        [BindProperty]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a station.")]
        public int SelectedStationId { get; set; }

        [BindProperty]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a package.")]
        public int SelectedPackageId { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Please select a time slot.")]
        public string SelectedSlot { get; set; } = string.Empty; // "yyyy-MM-ddTHH:mm:ss"

        [BindProperty]
        [Required(ErrorMessage = "Registration number is required.")]
        [StringLength(10, MinimumLength = 2, ErrorMessage = "Registration number must be 2-10 characters.")]
        [RegularExpression("^[A-Za-z0-9]+$", ErrorMessage = "Registration number may only contain letters and digits.")]
        public string RegistrationNumber { get; set; } = string.Empty;

        public List<Station> Stations { get; set; } = new();
        public List<Package> Packages { get; set; } = new();
        public List<UserVehicle> SavedCars { get; set; } = new();

        public TimeSpan OpeningTime { get; } = new(8, 0, 0);
        public TimeSpan ClosingTime { get; } = new(20, 0, 0);

        public async Task OnGetAsync()
        {
            Stations = await _stationService.GetStationsAsync();
            Packages = await _stationService.GetPackagesAsync();

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
            if (!string.IsNullOrEmpty(userId))
            {
                SavedCars = await _context.UserVehicles
                    .Where(v => v.UserId == userId)
                    .OrderBy(v => v.RegistrationNumber)
                    .ToListAsync();
            }

            if (Stations == null || !Stations.Any())
            {
                ModelState.AddModelError("", "No stations found. Please check your database.");
            }
        }

        // Returns the packages (with duration) offered by a station.
        public async Task<IActionResult> OnGetPackagesAsync(int stationId)
        {
            var packages = await _stationService.GetPackagesForStationAsync(stationId);
            var result = packages.Select(p => new { p.Id, p.Name, p.DurationMinutes, p.Price }).ToList();
            return new JsonResult(result);
        }

        // Returns the data the client needs to render the weekly booking calendar.
        public async Task<IActionResult> OnGetCalendarAsync(int stationId, int packageId)
        {
            var package = await _stationService.GetPackageAsync(packageId);
            if (package == null)
            {
                return NotFound();
            }

            var today = DateTime.Today;
            var from = today.Add(OpeningTime);
            var to = today.AddDays(CalendarDays).Add(ClosingTime);

            var bookings = await _bookingService.GetBookingsForStationAsync(stationId, from, to);

            var days = Enumerable.Range(0, CalendarDays)
                .Select(offset => today.AddDays(offset))
                .Select(d => new
                {
                    date = d.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                    weekday = d.ToString("ddd", CultureInfo.CurrentCulture),
                    day = d.Day,
                    month = d.ToString("MMM", CultureInfo.CurrentCulture),
                })
                .ToList();

            var booked = bookings.Select(b => new
            {
                start = b.Date.ToString("yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture),
                duration = b.DurationMinutes,
            }).ToList();

            return new JsonResult(new
            {
                durationMinutes = package.DurationMinutes,
                openingTime = OpeningTime.ToString(@"hh\:mm"),
                closingTime = ClosingTime.ToString(@"hh\:mm"),
                days,
                booked,
            });
        }

        public async Task<IActionResult> OnPostAsync()
        {
            _logger.LogInformation("OnPostAsync() started...");

            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (!TryParseSlot(out var slot))
            {
                return Page();
            }

            var stationExists = await _stationService.StationExistsAsync(SelectedStationId);
            if (!stationExists)
            {
                ModelState.AddModelError(nameof(SelectedStationId), "The selected station does not exist.");
                return Page();
            }

            var package = await _stationService.GetPackageAsync(SelectedPackageId);
            if (package == null)
            {
                ModelState.AddModelError(nameof(SelectedPackageId), "The selected package does not exist.");
                return Page();
            }

            var packageOffered = await _stationService.StationOffersPackageAsync(SelectedStationId, SelectedPackageId);
            if (!packageOffered)
            {
                ModelState.AddModelError(nameof(SelectedPackageId), "The selected package is not offered at this station.");
                return Page();
            }

            // Reject past slots and slots outside opening hours.
            if (slot < DateTime.Now)
            {
                ModelState.AddModelError("", "The selected time is in the past. Please choose a future time.");
                return Page();
            }

            if (slot.TimeOfDay < OpeningTime || slot.AddMinutes(package.DurationMinutes).TimeOfDay > ClosingTime)
            {
                ModelState.AddModelError("", "The selected time is outside opening hours.");
                return Page();
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;

            var result = await _bookingService.CreateBookingAsync(userId, new BookingInput
            {
                StationId = SelectedStationId,
                PackageId = SelectedPackageId,
                RegistrationNumber = RegistrationNumber.Trim().ToUpperInvariant(),
                Date = slot,
                DurationMinutes = package.DurationMinutes,
            });

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", result.Error ?? "Could not create the booking.");
                return Page();
            }

            TempData["SuccessMessage"] = "Your booking was successful!";
            return RedirectToPage("/MyBookingsPage");
        }

        private bool TryParseSlot(out DateTime slot)
        {
            slot = default;

            if (!DateTime.TryParseExact(SelectedSlot, "yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsed))
            {
                ModelState.AddModelError(nameof(SelectedSlot), "Invalid time slot.");
                return false;
            }

            slot = parsed;
            return true;
        }
    }
}
