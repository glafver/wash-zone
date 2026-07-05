# WashZone - Car Wash Booking Application

A comprehensive car wash booking system built with ASP.NET Core and Razor Pages.

## 📋 Prerequisites

Before starting the application, ensure you have the following installed:

- **.NET 8.0 SDK** - [Download](https://dotnet.microsoft.com/download/dotnet/8.0)
- **SQL Server** - Local or Azure SQL Database
- **Git** (optional, for cloning)

## 🔧 Database Setup

### Using Azure SQL Server (Current Configuration)
The application is configured to use Azure SQL Server. Ensure you have:
- Active Azure SQL Database connection
- Valid credentials in `appsettings.json`

### Using Local SQL Server
To switch to local SQL Server:

1. Open `appsettings.json`
2. Update the connection string:
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=WashZone;Trusted_Connection=true;MultipleActiveResultSets=true;"
}
```

3. Apply migrations:
```bash
dotnet ef database update
```

## 🚀 Getting Started

### 1. Clone or Open the Project
```bash
cd d:\workspace\WashZone
```

### 2. Restore Dependencies
```bash
dotnet restore
```

### 3. Apply Database Migrations
```bash
dotnet ef database update
```

### 4. Run the Application

**Note**: WashZone is a monolithic ASP.NET Core application. The frontend (Razor Pages) and backend run together. No separate frontend start needed!

#### Option A: Using dotnet CLI
```bash
dotnet run
```

#### Option B: Using Visual Studio
1. Open `WashZone.sln` in Visual Studio 2022+
2. Press `F5` or click **Start Debugging**

#### Option C: Using VS Code
```bash
dotnet run --urls "https://localhost:7000;http://localhost:5000"
```

### 5. Access the Application
- **Development**: `https://localhost:7000` (or the port shown in terminal)
- **Both frontend and backend start automatically** when you run the app
- Default browser opens automatically

## 📁 Project Structure

**This is a monolithic ASP.NET Core application** - frontend and backend run together as one application.

```
WashZone/
├── Program.cs                 # Application entry point
├── appsettings.json          # Configuration settings
├── appsettings.Development.json
├── WashZone.csproj           # Project file
├── WashZone.sln              # Solution file
├── Data/
│   ├── ApplicationDbContext.cs
│   └── SampleData.cs
├── Models/                    # Database models
│   ├── User.cs
│   ├── Station.cs
│   ├── Package.cs
│   ├── Booking.cs
│   └── Feature.cs
├── Pages/                     # Razor Pages (Frontend UI)
│   ├── Index.cshtml
│   ├── BookPage.cshtml
│   ├── MyBookingsPage.cshtml
│   ├── DetailsCarwash.cshtml
│   ├── AdminDashboard.cshtml
│   └── ...
├── Migrations/               # Database migrations
└── wwwroot/                  # Static files (CSS, JS, images)
```

## 🔐 Authentication

The application uses ASP.NET Core Identity:
- **Default Configuration**: No email confirmation required
- **Roles**: Support for role-based authorization
- **Login**: Available at `/Identity/Account/Login`
- **Registration**: Available at `/Identity/Account/Register`

### Sample Data
The application automatically seeds sample data on startup, including:
- Default users
- Car wash stations
- Service packages
- Features

## 🛠️ Development

### Useful Commands

#### Rebuild Solution
```bash
dotnet build
```

#### Clean Build
```bash
dotnet clean && dotnet build
```

#### Create Database Migration
```bash
dotnet ef migrations add MigrationName
```

#### Update Database with Migrations
```bash
dotnet ef database update
```

#### Remove Last Migration
```bash
dotnet ef migrations remove
```

#### View Database
- Use **SQL Server Management Studio (SSMS)** to connect and view the database
- Or use Visual Studio's **SQL Server Object Explorer**

## 🧪 Testing

To run tests (if configured):
```bash
dotnet test
```

## 📦 Dependencies

Key NuGet packages used:
- `Microsoft.AspNetCore.Identity.EntityFrameworkCore` - User authentication
- `Microsoft.EntityFrameworkCore.SqlServer` - Database ORM
- `Microsoft.AspNetCore.Identity.UI` - Identity UI components
- `Microsoft.EntityFrameworkCore.Tools` - EF Core tooling

## 🐛 Troubleshooting

### Connection String Issues
- Verify the connection string in `appsettings.json`
- Ensure database server is running and accessible
- Check firewall settings for Azure SQL

### Migration Errors
```bash
# Reset database (development only)
dotnet ef database drop --force
dotnet ef database update
```

### Port Already in Use
```bash
dotnet run --urls "https://localhost:7001;http://localhost:5001"
```

## 📝 Pages Overview

- **Index.cshtml** - Home page with available stations
- **BookPage.cshtml** - Book a car wash service
- **MyBookingsPage.cshtml** - View user's bookings
- **DetailsCarwash.cshtml** - Station details
- **AdminDashboard.cshtml** - Admin panel for managing bookings
- **EditBooking.cshtml** - Edit existing bookings

## 🌐 Configuration

Edit `appsettings.Development.json` for development-specific settings:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "your_connection_string_here"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

## 📄 License

This project is part of a study/work environment.

## 📞 Support

For issues or questions, check the error logs in the console or application output.

---

**Last Updated**: 2026-07-05
