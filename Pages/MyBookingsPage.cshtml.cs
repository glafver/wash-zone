using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using WashZone.Data;
using WashZone.Models;

namespace WashZone.Pages
{
    [Authorize]
    public class MyBookingsPageModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public MyBookingsPageModel(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public List<Booking> Bookings { get; set; } = new List<Booking>();
        public List<Station> Stations { get; set; } = new List<Station>();
        public List<Package> Packages { get; set; } = new List<Package>();

        public string SortOrder { get; set; } = string.Empty;
        public int? SelectedStationId { get; set; }
        public int? SelectedPackageId { get; set; }
        public string? RegNumberFilter { get; set; }

        public async Task<IActionResult> OnGetAsync(string sortOrder, int? stationId, int? packageId, string? regNumber)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Challenge();
            }

            SortOrder = sortOrder;
            SelectedStationId = stationId;
            SelectedPackageId = packageId;
            RegNumberFilter = regNumber;

            IQueryable<Booking> query = _context.Bookings
                .Include(b => b.Station)
                .Include(b => b.Package)
                .Where(b => b.UserId == userId);

            if (stationId.HasValue)
                query = query.Where(b => b.StationId == stationId);

            if (packageId.HasValue)
                query = query.Where(b => b.PackageId == packageId);

            if (!string.IsNullOrWhiteSpace(regNumber))
                query = query.Where(b => b.RegistrationNumber.Contains(regNumber));

            query = sortOrder == "desc"
                ? query.OrderByDescending(b => b.Date)
                : query.OrderBy(b => b.Date);

            Bookings = await query.ToListAsync();
            Stations = await _context.Stations.ToListAsync();
            Packages = await _context.Packages.ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null)
            {
                return NotFound();
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (booking.UserId != userId)
            {
                return Forbid();
            }

            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Booking successfully deleted!";
            return RedirectToPage();
        }
    }
}

