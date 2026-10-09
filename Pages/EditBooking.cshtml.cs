using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Security.Claims;
using WashZone.Models;
using WashZone.Services;

namespace WashZone.Pages
{
    [Authorize]
    public class EditBookingModel : PageModel
    {
        private readonly IBookingService _bookingService;
        private readonly IStationService _stationService;

        public EditBookingModel(IBookingService bookingService, IStationService stationService)
        {
            _bookingService = bookingService;
            _stationService = stationService;
        }

        [BindProperty]
        public Booking Booking { get; set; } = new Booking();

        public List<Station> Stations { get; set; } = new List<Station>();
        public List<Package> Packages { get; set; } = new List<Package>();

        [BindProperty]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a station.")]
        public int SelectedStationId { get; set; }
        [BindProperty]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a package.")]
        public int SelectedPackageId { get; set; }
        [BindProperty]
        [Range(1, 12, ErrorMessage = "Please select a month.")]
        public int SelectedMonth { get; set; }
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

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Stations = await _stationService.GetStationsAsync();
            Packages = await _stationService.GetPackagesAsync();

            var booking = await _bookingService.GetBookingAsync(id);
            if (booking == null)
            {
                return NotFound();
            }

            Booking = booking;

            // Pre-fill the form with the current booking data
            SelectedStationId = Booking.StationId;
            SelectedPackageId = Booking.PackageId;
            SelectedMonth = Booking.Date.Month;
            SelectedDay = Booking.Date.Day;
            SelectedTime = Booking.Date.ToString("HH:mm");
            RegistrationNumber = Booking.RegistrationNumber;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Stations = await _stationService.GetStationsAsync();
            Packages = await _stationService.GetPackagesAsync();

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
            if (!await _stationService.StationExistsAsync(SelectedStationId))
            {
                ModelState.AddModelError(nameof(SelectedStationId), "The selected station does not exist.");
                return Page();
            }

            if (!await _stationService.StationOffersPackageAsync(SelectedStationId, SelectedPackageId))
            {
                ModelState.AddModelError(nameof(SelectedPackageId), "The selected package is not offered at this station.");
                return Page();
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
            var isAdmin = User.IsInRole("Admin");

            var result = await _bookingService.UpdateBookingAsync(Booking.Id, userId, isAdmin, new BookingInput
            {
                StationId = SelectedStationId,
                PackageId = SelectedPackageId,
                RegistrationNumber = RegistrationNumber.Trim().ToUpperInvariant(),
                Date = finalBookingDate,
            });

            if (!result.Succeeded)
            {
                if (result.ErrorType == ServiceErrorType.NotFound) return NotFound();
                if (result.ErrorType == ServiceErrorType.Forbidden) return Forbid();

                ModelState.AddModelError("", result.Error ?? "Could not update the booking.");
                return Page();
            }

            TempData["SuccessMessage"] = "Booking updated successfully!";
            return RedirectToPage(isAdmin ? "/AdminDashboard" : "/MyBookingsPage");
        }

        /// <summary>
        /// Builds and validates the final booking <see cref="DateTime"/> from the selected
        /// month, day and time. Rolls over to next year when the selected month has already
        /// passed this year, and rejects dates/times in the past.
        /// </summary>
        private bool TryBuildBookingDate(out DateTime bookingDate)
        {
            bookingDate = default;

            if (!TimeSpan.TryParseExact(SelectedTime, @"hh\:mm", CultureInfo.InvariantCulture, out var time))
            {
                ModelState.AddModelError(nameof(SelectedTime), "Invalid time.");
                return false;
            }

            // If the selected month has already passed this year, assume the booking is for next year.
            int year = DateTime.Now.Year;
            if (SelectedMonth < DateTime.Now.Month)
            {
                year += 1;
            }

            int maxDay = DateTime.DaysInMonth(year, SelectedMonth);
            if (SelectedDay < 1 || SelectedDay > maxDay)
            {
                ModelState.AddModelError(nameof(SelectedDay), $"Invalid day for the selected month (max {maxDay}).");
                return false;
            }

            bookingDate = new DateTime(year, SelectedMonth, SelectedDay, time.Hours, time.Minutes, 0);

            if (bookingDate < DateTime.Now)
            {
                ModelState.AddModelError("", "The selected date and time is in the past. Please choose a future time.");
                return false;
            }

            return true;
        }
    }
}
