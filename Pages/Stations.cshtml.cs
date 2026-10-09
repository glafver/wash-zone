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

        public async Task OnGetAsync()
        {
            Stations = await _stationService.GetStationsAsync();
        }
    }
}
