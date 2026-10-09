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

        public IReadOnlyDictionary<string, string> FeatureImages { get; } = new Dictionary<string, string>
        {
            { "Exterior wash", "exterior-wash.png" },
            { "Interior cleaning", "interior-cleaning.png" },
            { "Waxing", "waxing.png" },
            { "Tire shine", "tire-shine.png" },
            { "Window cleaning", "window-cleaning.png" },
            { "Engine wash", "engine-wash.png" },
            { "Interior disinfection", "interior-disinfection.png" },
            { "Odor removal", "odor-removal.png" },
            { "Ceramic coating", "ceramic-coating.png" }
        };

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
