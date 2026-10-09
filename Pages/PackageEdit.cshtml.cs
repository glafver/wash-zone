using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WashZone.Models;
using WashZone.Services;

namespace WashZone.Pages
{
    [Authorize(Roles = "Admin")]
    public class PackageEditModel : PageModel
    {
        private readonly IPackageService _packageService;

        public PackageEditModel(IPackageService packageService)
        {
            _packageService = packageService;
        }

        [BindProperty]
        public Package Package { get; set; } = new();

        [BindProperty]
        public List<int> SelectedFeatureIds { get; set; } = new();

        public List<Feature> AllFeatures { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            AllFeatures = await _packageService.GetFeaturesAsync();

            if (id.HasValue && id > 0)
            {
                var package = await _packageService.GetPackageWithFeaturesAsync(id.Value);
                if (package == null) return NotFound();

                Package = package;
                SelectedFeatureIds = package.PackageFeatures.Select(pf => pf.FeatureId).ToList();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                AllFeatures = await _packageService.GetFeaturesAsync();
                return Page();
            }

            if (Package.Id == 0)
            {
                var created = await _packageService.CreatePackageAsync(new Package
                {
                    Name = Package.Name,
                    Price = Package.Price,
                    DurationMinutes = Package.DurationMinutes,
                });

                await _packageService.SetPackageFeaturesAsync(created.Id, SelectedFeatureIds);
                TempData["SuccessMessage"] = "Package created.";
                return RedirectToPage("/AdminPackages");
            }

            var result = await _packageService.UpdatePackageAsync(Package);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", result.Error ?? "Could not update the package.");
                AllFeatures = await _packageService.GetFeaturesAsync();
                return Page();
            }

            await _packageService.SetPackageFeaturesAsync(Package.Id, SelectedFeatureIds);
            TempData["SuccessMessage"] = "Package updated.";
            return RedirectToPage("/AdminPackages");
        }
    }
}
