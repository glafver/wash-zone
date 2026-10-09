using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WashZone.Models;
using WashZone.Services;

namespace WashZone.Pages
{
    public class DetailsCarwashModel : PageModel
    {
        private readonly IStationService _stationService;

        public DetailsCarwashModel(IStationService stationService)
        {
            _stationService = stationService;
        }

        public Station? Station { get; set; }
        public List<Package> AvailablePackages { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Station = await _stationService.GetStationDetailsAsync(id);

            if (Station == null)
            {
                return NotFound();
            }

            AvailablePackages = Station.StationPackages
                .Select(sp => sp.Package)
                .Distinct()
                .ToList();

            return Page();
        }
    }
}
