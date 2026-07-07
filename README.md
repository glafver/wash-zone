# WashZone - Car Wash Booking Application

A comprehensive car wash booking system built with **ASP.NET Core 8 Razor Pages**, **Entity Framework Core**, **SQL Server**, and **Docker Compose**.

The application is a monolithic ASP.NET Core application where the frontend (Razor Pages) and backend run together in one container, connected to a SQL Server database container.

---

# 📋 Prerequisites

Before starting the application, install:

* **Docker Desktop**
* **Git** (optional, for cloning)

No local SQL Server installation is required. The database runs inside Docker.

---

# 🐳 Docker Setup

The application consists of two containers:

```
              Browser
                 |
                 |
        ASP.NET Core Container
            (WashZone)
                 |
                 |
          SQL Server Container
```

Docker Compose manages both containers and their network connection.

---

# 🚀 Getting Started

## 1. Clone the project

```bash
git clone <repository-url>
cd WashZone
```

---

## 2. Configure environment variables

Create a file called:

```
.env
```

in the project root.

Example:

```env
ConnectionStrings__DefaultConnection=Server=sqlserver,1433;Database=WashZone_new_db;User Id=sa;Password=YourStrong!Pass123;TrustServerCertificate=True

GoogleMaps__ApiKey=your_google_maps_api_key
```

The `.env` file contains sensitive information and should **not** be committed to GitHub.

Add it to `.gitignore`:

```
.env
appsettings.Development.json
```

---

## 3. Start the application

Build and start all containers:

```bash
docker compose up --build
```

This starts:

* ASP.NET Core application
* SQL Server database

The application will be available at:

```
http://localhost:8080
```

---

## 4. Stop the application

```bash
docker compose down
```

To remove containers and the database volume:

```bash
docker compose down -v
```

⚠️ Removing volumes deletes the database data.

---

# 🗄️ Database

The application uses:

* Microsoft SQL Server 2022
* Entity Framework Core
* Code-first migrations

The database runs inside a Docker container and data is persisted using a Docker volume.

Database migrations are applied automatically when the application starts.

To manually create/update migrations:

Create migration:

```bash
dotnet ef migrations add MigrationName
```

Update database:

```bash
dotnet ef database update
```

---

# 📁 Project Structure

```
WashZone/
│
├── Dockerfile
├── docker-compose.yml
├── .env                         # Local secrets (not committed)
│
├── Program.cs                   # Application entry point
├── appsettings.json             # General configuration
├── appsettings.Development.json
├── WashZone.csproj
├── WashZone.sln
│
├── Data/
│   ├── ApplicationDbContext.cs
│   └── SampleData.cs
│
├── Models/
│   ├── User.cs
│   ├── Station.cs
│   ├── Package.cs
│   ├── Booking.cs
│   └── Feature.cs
│
├── Pages/                       # Razor Pages frontend
│   ├── Index.cshtml
│   ├── BookPage.cshtml
│   ├── MyBookingsPage.cshtml
│   ├── DetailsCarwash.cshtml
│   ├── AdminDashboard.cshtml
│   └── ...
│
├── Migrations/
│
└── wwwroot/                     # CSS, JS, images
```

---

# 🔐 Authentication

The application uses **ASP.NET Core Identity**.

Features:

* User registration
* User login
* Role-based authorization
* Admin dashboard
* Booking management

Login:

```
/Identity/Account/Login
```

Registration:

```
/Identity/Account/Register
```

---

# 🌱 Sample Data

On startup, the application seeds initial data:

* Users
* Car wash stations
* Service packages
* Features

---

# 🛠️ Useful Docker Commands

## Build containers

```bash
docker compose build
```

## Start containers in background

```bash
docker compose up -d
```

## View application logs

```bash
docker compose logs -f washzone
```

## View database logs

```bash
docker compose logs -f sqlserver
```

## Restart containers

```bash
docker compose restart
```

## Rebuild after changes

```bash
docker compose up --build
```

---

# 🧪 Testing

Run tests:

```bash
dotnet test
```

---

# 📦 Main Dependencies

* ASP.NET Core 8 Razor Pages
* Entity Framework Core
* SQL Server Provider
* ASP.NET Core Identity
* Docker
* Docker Compose

---

# 🌐 Configuration

Sensitive configuration is stored outside the application using environment variables.

Examples:

```
ConnectionStrings__DefaultConnection
GoogleMaps__ApiKey
```

These values are provided through Docker Compose locally and can be configured as environment variables in production hosting platforms.

---

# 🐛 Troubleshooting

## Database connection problems

Check:

* Docker Desktop is running
* SQL Server container is started
* `.env` exists
* Connection string uses:

```
Server=sqlserver,1433
```

inside Docker Compose.

---

## Container problems

Check logs:

```bash
docker compose logs -f
```

Rebuild everything:

```bash
docker compose down
docker compose build --no-cache
docker compose up
```

---

## Port already in use

Change the port mapping in:

```
docker-compose.yml
```

Example:

```yaml
ports:
  - "8081:8080"
```

---

# 📝 Pages Overview

* **Index.cshtml** - Available car wash stations
* **BookPage.cshtml** - Create booking
* **MyBookingsPage.cshtml** - User bookings
* **DetailsCarwash.cshtml** - Station details
* **AdminDashboard.cshtml** - Administration panel
* **EditBooking.cshtml** - Edit bookings

---

# 📄 License

This project is part of a study/work environment.

---

**Last Updated:** 2026-07-07
