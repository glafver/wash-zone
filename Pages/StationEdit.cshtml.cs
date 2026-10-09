using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WashZone.Models;
using WashZone.Services;

namespace WashZone.Pages
{
    [Authorize(Roles = "Admin")]
    public class StationEditModel : PageModel
    {
        private readonly IStationService _stationService;

        public StationEditModel(IStationService stationService)
        {
            _stationService = stationService;
        }

        [BindProperty]
        public Station Station { get; set; } = new();

        [BindProperty]
        public List<int> SelectedPackageIds { get; set; } = new();

        public List<Package> AllPackages { get; set; } = new();
        public List<IdentityUser> CurrentAdmins { get; set; } = new();
        public List<IdentityUser> AvailableUsers { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            await LoadDropdownsAsync();

            if (id.HasValue && id > 0)
            {
                var station = await _stationService.GetStationDetailsAsync(id.Value);
                if (station == null) return NotFound();

                Station = station;
                SelectedPackageIds = station.StationPackages.Select(sp => sp.PackageId).ToList();
                CurrentAdmins = await _stationService.GetStationAdminsAsync(id.Value);
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await LoadDropdownsAsync();
                return Page();
            }

            if (Station.Id == 0)
            {
                var created = await _stationService.CreateStationAsync(new Station
                {
                    Name = Station.Name,
                    Address = Station.Address,
                });

                await _stationService.SetStationPackagesAsync(created.Id, SelectedPackageIds);
                TempData["SuccessMessage"] = "Station created.";
                return RedirectToPage("/AdminStations");
            }

            var result = await _stationService.UpdateStationAsync(Station);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", result.Error ?? "Could not update the station.");
                await LoadDropdownsAsync();
                return Page();
            }

            await _stationService.SetStationPackagesAsync(Station.Id, SelectedPackageIds);
            TempData["SuccessMessage"] = "Station updated.";
            return RedirectToPage("/AdminStations");
        }

        public async Task<IActionResult> OnPostAssignAdminAsync(int id, string userId)
        {
            var result = await _stationService.AssignStationAdminAsync(id, userId);
            if (result.Succeeded) TempData["SuccessMessage"] = "Station admin assigned.";
            else TempData["ErrorMessage"] = result.Error;
            return RedirectToPage(new { id });
        }

        public async Task<IActionResult> OnPostRemoveAdminAsync(int id, string userId)
        {
            var result = await _stationService.RemoveStationAdminAsync(userId);
            if (result.Succeeded) TempData["SuccessMessage"] = "Station admin removed.";
            else TempData["ErrorMessage"] = result.Error;
            return RedirectToPage(new { id });
        }

        private async Task LoadDropdownsAsync()
        {
            AllPackages = await _stationService.GetPackagesAsync();
            AvailableUsers = await _stationService.GetAvailableAdminUsersAsync();

            if (Station.Id > 0)
            {
                CurrentAdmins = await _stationService.GetStationAdminsAsync(Station.Id);
            }
        }
    }
}
