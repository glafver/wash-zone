using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WashZone.Models;
using WashZone.Services;

namespace WashZone.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IStationService _stationService;

        public IndexModel(IStationService stationService)
        {
            _stationService = stationService;
        }

        public IList<Station> Stations { get; set; } = new List<Station>();
        public IList<Package> Packages { get; set; } = new List<Package>();
        public IList<Feature> Features { get; set; } = new List<Feature>();

        [BindProperty(SupportsGet = true)]
        public int? SelectedPackageId { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            Packages = await _stationService.GetPackagesAsync();
            Features = await _stationService.GetFeaturesAsync();
            Stations = await _stationService.GetStationsAsync(SelectedPackageId);

            return Page();
        }

        public IActionResult OnPostBookPage()
        {
            return RedirectToPage("BookPage");
        }

        public async Task<IActionResult> OnGetStationsAsync(int? packageId)
        {
            var stations = await _stationService.GetStationsAsync(packageId);
            return Partial("_StationCards", stations);
        }
    }
}
