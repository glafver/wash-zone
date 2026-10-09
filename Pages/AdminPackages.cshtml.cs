using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WashZone.Models;
using WashZone.Services;

namespace WashZone.Pages
{
    [Authorize(Roles = "Admin")]
    public class AdminPackagesModel : PageModel
    {
        private readonly IPackageService _packageService;

        public AdminPackagesModel(IPackageService packageService)
        {
            _packageService = packageService;
        }

        public List<Package> Packages { get; set; } = new();

        public async Task OnGetAsync()
        {
            Packages = await _packageService.GetPackagesAsync();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var result = await _packageService.DeletePackageAsync(id);
            if (result.Succeeded)
                TempData["SuccessMessage"] = "Package deleted.";
            else
                TempData["ErrorMessage"] = result.Error;
            return RedirectToPage();
        }
    }
}
