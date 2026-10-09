using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WashZone.Data;
using WashZone.Models;
using WashZone.Services;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// Connection string (set via environment variable or appsettings.Development.json locally,
// or through docker-compose's environment section in production)
var connectionString =
	builder.Configuration.GetConnectionString("DefaultConnection")
	?? throw new InvalidOperationException(
		"Connection string 'DefaultConnection' is not configured. " +
		"Set ConnectionStrings__DefaultConnection in your environment or appsettings.Development.json.");

// DB (retries transient SQL errors so the app can wait for SQL Server to become ready in Docker)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
	options.UseSqlServer(connectionString, sql => sql.EnableRetryOnFailure()));

// Identity
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
	options.SignIn.RequireConfirmedAccount = false;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddRazorPages();

// Localization (English + Swedish)
// The SDK embeds "Resources/SharedResource.resx" as "WashZone.SharedResource"
// (root namespace + file name), so an empty ResourcesPath matches that name.
builder.Services.AddLocalization();

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
	var supportedCultures = new[] { new CultureInfo("en"), new CultureInfo("sv") };
	options.DefaultRequestCulture = new RequestCulture("en");
	options.SupportedCultures = supportedCultures;
	options.SupportedUICultures = supportedCultures;
	options.RequestCultureProviders = new List<IRequestCultureProvider>
	{
		new QueryStringRequestCultureProvider(),
		new CookieRequestCultureProvider(),
		new AcceptLanguageHeaderRequestCultureProvider(),
	};
});

builder.Services.AddScoped<IStationService, StationService>();
builder.Services.AddScoped<IBookingService, BookingService>();

var app = builder.Build();

// DB + Seed
using (var scope = app.Services.CreateScope())
{
	var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
	var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
	var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

	context.Database.Migrate();

	SampleData.SeedData(context, userManager, roleManager);
}

if (app.Environment.IsDevelopment())
{
	app.UseMigrationsEndPoint();
}
else
{
	app.UseExceptionHandler("/Error");
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRequestLocalization();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

// Language switcher: persists the chosen culture in a cookie and redirects back.
app.MapPost("/set-language", ([FromForm] string? culture, [FromForm] string? returnUrl, HttpResponse response) =>
{
	if (culture is "en" or "sv")
	{
		response.Cookies.Append(
			CookieRequestCultureProvider.DefaultCookieName,
			CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
			new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1), HttpOnly = true });
	}

	// Only allow same-site redirects to avoid open redirect.
	var target = string.IsNullOrWhiteSpace(returnUrl) || !returnUrl.StartsWith('/') ? "/" : returnUrl;
	return Results.LocalRedirect(target);
}).DisableAntiforgery();

var logger = app.Services.GetRequiredService<ILogger<Program>>();
logger.LogInformation("🚀 WashZone is running on http://localhost:8080");

app.Run();
