using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System.Security.Claims;
using WashZone.Data;
using WashZone.Models;

namespace WashZone.Pages
{
    [Authorize]
    public class ProfileModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ApplicationDbContext _context;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public ProfileModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ApplicationDbContext context,
            IStringLocalizer<SharedResource> localizer)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _localizer = localizer;
        }

        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public List<UserVehicle> Vehicles { get; set; } = new();
        public bool CanDeleteAccount { get; set; }

        [BindProperty]
        public string NewPhoneNumber { get; set; } = string.Empty;

        [BindProperty]
        public string NewRegistrationNumber { get; set; } = string.Empty;

        [BindProperty]
        public string CurrentPassword { get; set; } = string.Empty;

        [BindProperty]
        public string NewPassword { get; set; } = string.Empty;

        [BindProperty]
        public string ConfirmPassword { get; set; } = string.Empty;

        public async Task<IActionResult> OnGetAsync()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return Challenge();

            await LoadAsync(userId);
            return Page();
        }

        public async Task<IActionResult> OnPostUpdatePhoneAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            user.PhoneNumber = NewPhoneNumber?.Trim();
            await _userManager.UpdateAsync(user);
            TempData["SuccessMessage"] = _localizer["PhoneUpdated"].Value;
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostAddVehicleAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var reg = (NewRegistrationNumber ?? string.Empty).Trim().ToUpperInvariant();
            if (string.IsNullOrEmpty(reg))
            {
                TempData["ErrorMessage"] = _localizer["VehicleExists"].Value;
                return RedirectToPage();
            }

            var exists = await _context.UserVehicles.AnyAsync(v => v.UserId == user.Id && v.RegistrationNumber == reg);
            if (exists)
            {
                TempData["ErrorMessage"] = _localizer["VehicleExists"].Value;
            }
            else
            {
                _context.UserVehicles.Add(new UserVehicle { UserId = user.Id, RegistrationNumber = reg });
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = _localizer["VehicleAdded"].Value;
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostRemoveVehicleAsync(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var vehicle = await _context.UserVehicles.FirstOrDefaultAsync(v => v.Id == id && v.UserId == user.Id);
            if (vehicle != null)
            {
                _context.UserVehicles.Remove(vehicle);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = _localizer["VehicleRemoved"].Value;
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostChangePasswordAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            if (NewPassword != ConfirmPassword)
            {
                TempData["ErrorMessage"] = _localizer["PasswordMismatch"].Value;
                return RedirectToPage();
            }

            var result = await _userManager.ChangePasswordAsync(user, CurrentPassword, NewPassword);
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = _localizer["PasswordChanged"].Value;
            }
            else
            {
                TempData["ErrorMessage"] = result.Errors.FirstOrDefault()?.Description ?? _localizer["PasswordMismatch"].Value;
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteAccountAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            // The main admin cannot delete their own account.
            if (user.Email == "admin@washzone.se")
            {
                return Forbid();
            }

            var userId = user.Id;

            // Remove related data before deleting the user.
            var vehicles = await _context.UserVehicles.Where(v => v.UserId == userId).ToListAsync();
            _context.UserVehicles.RemoveRange(vehicles);

            var stationAdmin = await _context.StationAdmins.FirstOrDefaultAsync(sa => sa.UserId == userId);
            if (stationAdmin != null)
            {
                _context.StationAdmins.Remove(stationAdmin);
                await _userManager.RemoveFromRoleAsync(user, "StationAdmin");
            }

            var bookings = await _context.Bookings.Where(b => b.UserId == userId).ToListAsync();
            _context.Bookings.RemoveRange(bookings);

            await _context.SaveChangesAsync();

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                TempData["ErrorMessage"] = _localizer["DeleteAccountError"].Value;
                return RedirectToPage();
            }

            await _signInManager.SignOutAsync();
            return RedirectToPage("/Index");
        }

        private async Task LoadAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                Email = user.Email ?? string.Empty;
                PhoneNumber = user.PhoneNumber ?? string.Empty;
                NewPhoneNumber = PhoneNumber;
                CanDeleteAccount = user.Email != "admin@washzone.se";
            }

            Vehicles = await _context.UserVehicles
                .Where(v => v.UserId == userId)
                .OrderBy(v => v.RegistrationNumber)
                .ToListAsync();
        }
    }
}
