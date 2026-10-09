using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WashZone.Models;
using WashZone.Services;

namespace WashZone.Pages
{
    [Authorize(Roles = "Admin")]
    public class AdminDashboardModel : PageModel
    {
        private readonly IBookingService _bookingService;
        private readonly IStationService _stationService;

        public AdminDashboardModel(IBookingService bookingService, IStationService stationService)
        {
            _bookingService = bookingService;
            _stationService = stationService;
        }

        public List<Booking> Bookings { get; set; } = new List<Booking>();
        public List<Station> Stations { get; set; } = new List<Station>();
        public List<Package> Packages { get; set; } = new List<Package>();

        public string SortOrder { get; set; } = string.Empty;
        public int? SelectedStationId { get; set; }
        public int? SelectedPackageId { get; set; }
        public string? RegNumberFilter { get; set; }
        public string? PhoneNumberFilter { get; set; }

        public async Task<IActionResult> OnGetAsync(string sortOrder = "desc", int? stationId = null, int? packageId = null, string? regNumber = null, string? phoneNumber = null)
        {
            SortOrder = sortOrder;
            SelectedStationId = stationId;
            SelectedPackageId = packageId;
            RegNumberFilter = regNumber;
            PhoneNumberFilter = phoneNumber;

            Stations = await _stationService.GetStationsAsync();
            Packages = await _stationService.GetPackagesAsync();

            // Default sort order is descending ("newest first").
            var descending = sortOrder != "asc";

            Bookings = await _bookingService.GetAllBookingsAsync(descending, stationId, packageId, regNumber, phoneNumber);

            return Page();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var result = await _bookingService.DeleteBookingAsync(id, string.Empty, isAdmin: true);

            if (!result.Succeeded)
            {
                return result.ErrorType switch
                {
                    ServiceErrorType.NotFound => (IActionResult)NotFound(),
                    _ => RedirectToPage(),
                };
            }

            TempData["SuccessMessage"] = "Booking successfully deleted!";
            return RedirectToPage();
        }
    }
}
