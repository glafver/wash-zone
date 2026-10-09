using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using WashZone.Models;
using WashZone.Services;

namespace WashZone.Pages
{
    [Authorize(Roles = "StationAdmin")]
    public class StationDashboardModel : PageModel
    {
        private readonly IBookingService _bookingService;
        private readonly IStationService _stationService;

        public StationDashboardModel(IBookingService bookingService, IStationService stationService)
        {
            _bookingService = bookingService;
            _stationService = stationService;
        }

        public Station? Station { get; set; }
        public List<Booking> Bookings { get; set; } = new();
        public string SortOrder { get; set; } = string.Empty;

        public async Task<IActionResult> OnGetAsync(string sortOrder = "desc")
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
            var stationId = await _stationService.GetStationIdForAdminAsync(userId);

            if (stationId == null)
            {
                return NotFound();
            }

            Station = await _stationService.GetStationDetailsAsync(stationId.Value);
            if (Station == null)
            {
                return NotFound();
            }

            SortOrder = sortOrder;
            var descending = sortOrder != "asc";
            Bookings = await _bookingService.GetAllBookingsAsync(descending, stationId.Value, null, null, null);

            return Page();
        }
    }
}
