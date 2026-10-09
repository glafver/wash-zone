using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WashZone.Models;
using WashZone.Services;

namespace WashZone.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IStationService _stationService;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IConfiguration _configuration;

        public IndexModel(
            IStationService stationService,
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            IConfiguration configuration)
        {
            _stationService = stationService;
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }

        public IList<Station> Stations { get; set; } = new List<Station>();
        public IList<Package> Packages { get; set; } = new List<Package>();
        public string GoogleMapsApiKey { get; set; } = string.Empty;

        [BindProperty(SupportsGet = true)]
        public int? SelectedPackageId { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            GoogleMapsApiKey = _configuration["GoogleMaps:ApiKey"]
                ?? _configuration["GoogleMapsApiKey"]
                ?? "";

            Packages = await _stationService.GetPackagesAsync();
            Stations = await _stationService.GetStationsAsync(SelectedPackageId);

            if (_signInManager.IsSignedIn(User))
            {
                var user = await _userManager.GetUserAsync(User);
                if (user != null && await _userManager.IsInRoleAsync(user, "Admin"))
                {
                    return RedirectToPage("/AdminDashboard");
                }
            }

            return Page();
        }

        public IActionResult OnPostBookPage()
        {
            return RedirectToPage("BookPage");
        }
    }
}
