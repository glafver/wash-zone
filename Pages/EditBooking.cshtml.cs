using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using WashZone.Data;
using WashZone.Models;

namespace WashZone.Pages
{
    public class EditBookingModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public EditBookingModel(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [BindProperty]
        public Booking Booking { get; set; } = new Booking();

        public List<Station> Stations { get; set; } = new List<Station>();
        public List<Package> Packages { get; set; } = new List<Package>();
        public List<Booking> Bookings { get; set; } = new List<Booking>();

        [BindProperty]
        public int SelectedStationId { get; set; }
        [BindProperty]
        public int SelectedPackageId { get; set; }
        [BindProperty]
        public string SelectedMonth { get; set; } = string.Empty;
        [BindProperty]
        public int SelectedDay { get; set; }
        [BindProperty]
        public string SelectedTime { get; set; } = string.Empty;

        [BindProperty]
        public string RegistrationNumber { get; set; } = string.Empty;
        public async Task<IActionResult> OnGetAsync(int id)
        {
            // Load stations and packages when the page loads
            Stations = await _context.Stations.ToListAsync();
            Packages = await _context.Packages.ToListAsync();

            // Fetch the booking from the database
            var booking = await _context.Bookings.FindAsync(id);

            if (booking == null)
            {
                return NotFound();
            }

            Booking = booking;

            // Pre-fill the form with the current booking data
            SelectedStationId = Booking.StationId;
            SelectedPackageId = Booking.PackageId;
            SelectedMonth = Booking.Date.ToString("MMMM");
            SelectedDay = Booking.Date.Day;
            SelectedTime = Booking.Date.ToString("HH:mm");
            RegistrationNumber = Booking.RegistrationNumber;

            return Page();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            Stations = await _context.Stations.ToListAsync();
            Packages = await _context.Packages.ToListAsync();

            try
            {
                // Fetch the existing booking from the database
                var existingBooking = await _context.Bookings.FindAsync(Booking.Id);

                if (existingBooking == null)
                {
                    return NotFound();
                }

                // Ensure the current user owns the booking
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (existingBooking.UserId != userId && !User.IsInRole("Admin"))
                {
                    return Forbid();
                }
                int monthNumber = DateTime.ParseExact(SelectedMonth, "MMMM", System.Globalization.CultureInfo.InvariantCulture).Month;
                DateTime selectedTimeParsed = DateTime.ParseExact(SelectedTime, "HH:mm", System.Globalization.CultureInfo.InvariantCulture);
                DateTime finalBookingDate = new DateTime(DateTime.Now.Year, monthNumber, SelectedDay, selectedTimeParsed.Hour, selectedTimeParsed.Minute, 0);

                // Update the booking properties
                existingBooking.StationId = SelectedStationId;
                existingBooking.PackageId = SelectedPackageId;
                existingBooking.Date = finalBookingDate;
                existingBooking.RegistrationNumber = RegistrationNumber;

                // Mark the entity as modified and save changes
                _context.Attach(existingBooking).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Booking updated successfully!";
                if (User.IsInRole("Admin"))
                {
                    return RedirectToPage("/AdminDashboard");
                }
                else
                {
                    return RedirectToPage("/MyBookingsPage");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while updating the booking. Please try again.");
                Console.WriteLine($"Exception: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                return Page();
            }
        }
    }
}
