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

        public IndexModel(
            IStationService stationService,
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager)
        {
            _stationService = stationService;
            _userManager = userManager;
            _signInManager = signInManager;
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
