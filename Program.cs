using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WashZone.Data;
using WashZone.Models;

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

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

var logger = app.Services.GetRequiredService<ILogger<Program>>();
logger.LogInformation("🚀 WashZone is running on http://localhost:8080");

app.Run();