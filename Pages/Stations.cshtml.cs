using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WashZone.Models;
using WashZone.Services;

namespace WashZone.Pages
{
    public class StationsModel : PageModel
    {
        private readonly IStationService _stationService;

        public StationsModel(IStationService stationService)
        {
            _stationService = stationService;
        }

        public IList<Station> Stations { get; set; } = new List<Station>();
        public IList<Package> Packages { get; set; } = new List<Package>();

        [BindProperty(SupportsGet = true)]
        public int? SelectedPackageId { get; set; }

        public async Task OnGetAsync()
        {
            Packages = await _stationService.GetPackagesAsync();
            Stations = await _stationService.GetStationsAsync(SelectedPackageId);
        }

        public async Task<IActionResult> OnGetStationsAsync(int? packageId)
        {
            var stations = await _stationService.GetStationsAsync(packageId);
            return Partial("_StationCards", stations);
        }
    }
}
