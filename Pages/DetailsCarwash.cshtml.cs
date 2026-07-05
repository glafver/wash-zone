using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WashZone.Data;
using WashZone.Models;

namespace WashZone.Pages
{
    public class DetailsCarwashModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DetailsCarwashModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IReadOnlyDictionary<string, string> FeatureImages { get; } = new Dictionary<string, string>
        {
            { "Utvändig tvätt", "exterior-wash.png" },
            { "Invändig rengöring", "interior-cleaning.png" },
            { "Vaxning", "waxing.png" },
            { "Däckglans", "tire-shine.png" },
            { "Fönsterputs", "window-cleaning.png" },
            { "Motortvätt", "engine-wash.png" },
            { "Interiör desinficering", "interior-disinfection.png" },
            { "Luktsanering", "odor-removal.png" },
            { "Keramisk beläggning", "ceramic-coating.png" }
        };

        public Station? Station { get; set; }
        public List<Package> AvailablePackages { get; set; } = new();

        public IActionResult OnGet(int id)
        {
            Station = _context.Stations
                .Include(s => s.StationPackages)
                .ThenInclude(sp => sp.Package)
                .ThenInclude(p => p.PackageFeatures) // Inkludera kopplingen till features
                .ThenInclude(pf => pf.Feature) // Inkludera själva feature-data
                .FirstOrDefault(s => s.Id == id);

            if (Station == null)
            {
                return NotFound();
            }

            AvailablePackages = Station.StationPackages
                .Select(sp => sp.Package)
                .Distinct()
                .ToList();

            return Page();
        }
    }
}

