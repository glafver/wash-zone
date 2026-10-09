using Microsoft.AspNetCore.Mvc.RazorPages;
using WashZone.Models;
using WashZone.Services;

namespace WashZone.Pages
{
    public class PackagesModel : PageModel
    {
        private readonly IPackageService _packageService;

        public PackagesModel(IPackageService packageService)
        {
            _packageService = packageService;
        }

        public List<Package> Packages { get; set; } = new();

        public async Task OnGetAsync()
        {
            Packages = await _packageService.GetPackagesWithFeaturesAsync();
        }
    }
}
