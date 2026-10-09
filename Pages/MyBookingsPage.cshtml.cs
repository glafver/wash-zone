using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using WashZone.Models;
using WashZone.Services;

namespace WashZone.Pages
{
    [Authorize]
    public class MyBookingsPageModel : PageModel
    {
        private readonly IBookingService _bookingService;
        private readonly IStationService _stationService;

        public MyBookingsPageModel(IBookingService bookingService, IStationService stationService)
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

        public async Task<IActionResult> OnGetAsync(string sortOrder, int? stationId, int? packageId, string? regNumber)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Challenge();
            }

            SortOrder = sortOrder;
            SelectedStationId = stationId;
            SelectedPackageId = packageId;
            RegNumberFilter = regNumber;

            // Default sort order is ascending ("oldest first").
            var descending = sortOrder == "desc";

            Bookings = await _bookingService.GetUserBookingsAsync(userId, descending, stationId, packageId, regNumber);
            Stations = await _stationService.GetStationsAsync();
            Packages = await _stationService.GetPackagesAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;

            var result = await _bookingService.DeleteBookingAsync(id, userId, isAdmin: false);

            if (!result.Succeeded)
            {
                return result.ErrorType switch
                {
                    ServiceErrorType.NotFound => (IActionResult)NotFound(),
                    ServiceErrorType.Forbidden => Forbid(),
                    _ => RedirectToPage(),
                };
            }

            TempData["SuccessMessage"] = "Booking successfully deleted!";
            return RedirectToPage();
        }
    }
}
