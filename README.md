# WashZone — Car Wash Booking Application

A full-stack car wash booking system built with **ASP.NET Core 8 Razor Pages**, **Entity Framework Core**, **SQL Server**, **ASP.NET Core Identity** and **Docker Compose**.

Users can browse car wash stations on a map, view available wash packages and their features, and book a wash. An admin dashboard gives an overview of all bookings.

---

## ✨ Features

- User registration / login (ASP.NET Core Identity with `Admin` and `User` roles)
- List of car wash stations with a Google Maps view (Malmö, Sweden)
- Filter stations by wash package
- Station details: available packages and included features
- Booking flow: station → package → date & time → registration number
- "My bookings" page with sorting and filtering
- Edit and delete your own bookings
- Admin dashboard: view all bookings, filter by station / package / registration number / phone number, edit and delete
- Bilingual UI (English / Swedish) with a language switcher
- Seeded sample data (stations, packages, features, users, bookings)
- CI build pipeline (GitHub Actions)

## 🧑‍💻 Demo accounts

Seeded on first start:

| Role  | Email               | Password    |
| ----- | ------------------- | ----------- |
| Admin | `admin@washzone.se` | `Admin123!` |
| User  | `user1@test.se`     | `User123!`  |

> ⚠️ These are demo credentials. In a real deployment, move them to configuration and change them.

## 🛠 Tech stack

- .NET 8 / ASP.NET Core Razor Pages
- Entity Framework Core 8 (code-first migrations)
- SQL Server 2022
- ASP.NET Core Identity
- Google Maps JavaScript API
- Docker & Docker Compose

---

## 🚀 Getting started (Docker — recommended)

The app runs as two containers: the ASP.NET Core app and a SQL Server database.

### 1. Configure environment variables

```bash
cp .env.example .env
```

Edit `.env` and set a real SQL Server password (and optionally your Google Maps API key):

```
MSSQL_SA_PASSWORD=YourStrong!Pass123
GOOGLE_MAPS_API_KEY=
```

The `.env` file is git-ignored and never committed.

### 2. Start the application

```bash
docker compose up --build
```

The database migrations are applied automatically on startup and the sample data is seeded.

The app is then available at:

```
http://localhost:8080
```

### 3. Stop the application

```bash
docker compose down          # stop containers, keep the database volume
docker compose down -v       # also remove the database data
```

---

## 💻 Local development (without Docker for the app)

If you prefer to run the app with `dotnet run`, you only need the database in Docker:

```bash
docker compose up sqlserver -d
```

Then create `appsettings.Development.json` (git-ignored) with:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=WashZone;User Id=sa;Password=YourStrong!Pass123;TrustServerCertificate=True"
  },
  "GoogleMaps": {
    "ApiKey": ""
  }
}
```

Finally:

```bash
dotnet run
```

The app will start on the URL defined in `Properties/launchSettings.json` (by default `http://localhost:5041`).

---

## 🗄 Database & migrations

The project uses EF Core **code-first migrations** (committed under `Migrations/`). They are applied automatically at startup via `context.Database.Migrate()`.

To add a new migration, restore the local EF tool and run:

```bash
dotnet tool restore
dotnet tool run dotnet-ef migrations add <MigrationName>
```

> The EF tool is pinned to `8.0.11` in `.config/dotnet-tools.json` to match the EF Core version used by the project.

---

## 📁 Project structure

```
WashZone/
├── .github/workflows/ci.yml      # CI build pipeline
├── docker-compose.yml            # app + SQL Server
├── Dockerfile                    # multi-stage .NET build
├── .env.example                  # environment variable template
│
├── Program.cs                    # entry point (DI, Identity, migrations, seeding)
├── appsettings.json
├── WashZone.csproj / .sln
│
├── Data/
│   ├── ApplicationDbContext.cs   # DbContext + EF model configuration
│   └── SampleData.cs             # seed data
│
├── Models/                       # domain entities
│   ├── User.cs
│   ├── Station.cs
│   ├── Package.cs
│   ├── Feature.cs
│   ├── Booking.cs
│   ├── StationPackage.cs         # many-to-many join
│   └── PackageFeature.cs         # many-to-many join
│
├── Migrations/                   # EF Core migrations
│
├── Pages/                        # Razor Pages UI + code-behind
│   ├── Index.cshtml              # station list + map
│   ├── DetailsCarwash.cshtml     # station details
│   ├── BookPage.cshtml           # create booking
│   ├── MyBookingsPage.cshtml     # user's bookings
│   ├── EditBooking.cshtml        # edit booking
│   └── AdminDashboard.cshtml     # admin panel
│
└── wwwroot/                      # static assets (CSS, JS, images, lib)
```

---

## 🔧 CI

GitHub Actions runs a build on every push and pull request to `main` (`.github/workflows/ci.yml`).

---

## 📝 License

This project is part of a study / portfolio environment.
