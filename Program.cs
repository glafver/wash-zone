using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WashZone.Data;
using WashZone.Models;

// Fixed port for WashZone
const int fixedPort = 5000;

var builder = WebApplication.CreateBuilder(args);

// Configure to use fixed port
builder.WebHost.UseUrls($"http://localhost:{fixedPort}");

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
	options.UseSqlServer(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
	.AddRoles<IdentityRole>()
	.AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddRazorPages();

var app = builder.Build();

// Seed Data
using (var scope = app.Services.CreateScope())
{
	var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
	var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
	var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
	SampleData.SeedData(context, userManager, roleManager);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseMigrationsEndPoint();
}
else
{
	app.UseExceptionHandler("/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

// Log clickable localhost links with fixed port
var logger = app.Services.GetRequiredService<ILogger<Program>>();
var appUrl = $"http://localhost:{fixedPort}";

logger.LogInformation("╔════════════════════════════════════════════════════════╗");
logger.LogInformation("║           🚀 WashZone is Running!                     ║");
logger.LogInformation("╠════════════════════════════════════════════════════════╣");
logger.LogInformation("║  📍 Open in browser: {url}            ║", appUrl);
logger.LogInformation("╚════════════════════════════════════════════════════════╝");

app.Run();

