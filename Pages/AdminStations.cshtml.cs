using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WashZone.Models;
using WashZone.Services;

namespace WashZone.Pages
{
    [Authorize(Roles = "Admin")]
    public class AdminStationsModel : PageModel
    {
        private readonly IStationService _stationService;

        public AdminStationsModel(IStationService stationService)
        {
            _stationService = stationService;
        }

        public List<Station> Stations { get; set; } = new();
        public Dictionary<int, string> AdminNames { get; set; } = new();

        public async Task OnGetAsync()
        {
            Stations = await _stationService.GetStationsAsync();
            AdminNames = new Dictionary<int, string>();
            foreach (var station in Stations)
            {
                var admins = await _stationService.GetStationAdminsAsync(station.Id);
                AdminNames[station.Id] = string.Join(", ", admins.Select(a => a.UserName));
            }
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var result = await _stationService.DeleteStationAsync(id);
            if (result.Succeeded)
                TempData["SuccessMessage"] = "Station deleted.";
            else
                TempData["ErrorMessage"] = result.Error;
            return RedirectToPage();
        }
    }
}
