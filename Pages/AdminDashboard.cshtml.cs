using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WashZone.Data;
using WashZone.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace WashZone.Pages
{
    [Authorize(Roles = "Admin")]
    public class AdminDashboardModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public AdminDashboardModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Booking> Bookings { get; set; } = new List<Booking>();
        public List<Station> Stations { get; set; } = new List<Station>();
        public List<Package> Packages { get; set; } = new List<Package>();
        public string SortOrder { get; set; } = string.Empty;
        public int? SelectedStationId { get; set; }
        public int? SelectedPackageId { get; set; }
        public string? RegNumberFilter { get; set; }
        public string? PhoneNumberFilter { get; set; }

        public async Task<IActionResult> OnGetAsync(string sortOrder = "desc", int? stationId = null, int? packageId = null, string? regNumber = null, string? phoneNumber = null)
        {
            if (!User.IsInRole("Admin"))
            {
                return RedirectToPage("/Index");
            }

            SortOrder = sortOrder;
            SelectedStationId = stationId;
            SelectedPackageId = packageId;
            RegNumberFilter = regNumber;

            Stations = await _context.Stations.ToListAsync();
            Packages = await _context.Packages.ToListAsync();

            IQueryable<Booking> bookingsQuery = _context.Bookings
                .Include(b => b.Station)
                .Include(b => b.Package)
                .Include(b => b.User);

            if (stationId.HasValue)
            {
                bookingsQuery = bookingsQuery.Where(b => b.Station.Id == stationId);
            }

            if (packageId.HasValue)
            {
                bookingsQuery = bookingsQuery.Where(b => b.Package.Id == packageId);
            }

            if (!string.IsNullOrEmpty(regNumber))
            {
                bookingsQuery = bookingsQuery.Where(b => b.RegistrationNumber.Contains(regNumber));
            }

            if (!string.IsNullOrWhiteSpace(phoneNumber))
                bookingsQuery = bookingsQuery.Where(b => b.User != null && b.User.PhoneNumber != null && b.User.PhoneNumber.Contains(phoneNumber));

            bookingsQuery = sortOrder == "asc"
                ? bookingsQuery.OrderBy(b => b.Date)
                : bookingsQuery.OrderByDescending(b => b.Date);

            Bookings = await bookingsQuery.ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            if (!User.IsInRole("Admin"))
            {
                return RedirectToPage("/Index");
            }

            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null)
            {
                return NotFound();
            }

            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Booking successfully deleted!";
            return RedirectToPage();
        }
    }
}

