# WashZone — Car Wash Booking Application

A full-stack car wash booking system built with **ASP.NET Core 8 Razor Pages**, **Entity Framework Core**, **PostgreSQL (Supabase)**, **ASP.NET Core Identity** and **Docker**.

Users can browse car wash stations on a map, view available wash packages and their features, and book a wash. An admin dashboard gives an overview of all bookings.

---

## ✨ Features

- User registration / login (ASP.NET Core Identity with `Admin`, `StationAdmin` and `User` roles)
- List of car wash stations on an interactive map (Leaflet + OpenStreetMap, Malmö, Sweden)
- Filter stations by wash package
- Station details: available packages and included features
- Booking flow: station → package → weekly calendar → registration number
- Saved cars: remember registration numbers for quick re-booking
- "My bookings" page with sorting and filtering
- Edit and delete your own bookings
- User profile: email, phone, saved cars, change password, delete account
- Admin dashboard: view all bookings, filter, edit and delete
- Station admin dashboard: manage bookings of a single station
- Manage stations and packages (admin CRUD, assign station admins)
- Bilingual UI (English / Swedish) with a language switcher
- Seeded sample data (stations, packages, features, users, bookings)
- CI build pipeline (GitHub Actions)

## 🧑‍💻 Demo accounts

Seeded on first start:

| Role          | Email                      | Password    |
| ------------- | -------------------------- | ----------- |
| Admin         | `admin@washzone.se`        | `Admin123!` |
| Station admin | `stationadmin@washzone.se` | `Admin123!` |
| User          | `user1@test.se`            | `User123!`  |

> ⚠️ These are demo credentials. In a real deployment, move them to configuration and change them.

## 🛠 Tech stack

- .NET 8 / ASP.NET Core Razor Pages
- Entity Framework Core 8 (code-first migrations, Npgsql)
- PostgreSQL (hosted on Supabase)
- ASP.NET Core Identity
- Leaflet + OpenStreetMap (maps)
- Docker

---

## 🚀 Getting started (Docker — recommended)

The app runs in a single container; the database is a free hosted **PostgreSQL** on [Supabase](https://supabase.com).

### 1. Create a Supabase project

1. Sign up / log in at [supabase.com](https://supabase.com).
2. Create a new project and choose a region close to you.
3. In **Project Settings → Database**, copy the **Session pooler** connection string (port `5432`, host `*.pooler.supabase.com`).

### 2. Configure environment variables

```bash
cp .env.example .env
```

Edit `.env` and set your Supabase connection string:

```
ConnectionStrings__DefaultConnection=postgresql://postgres.YOUR-PROJECT-REF:YOUR-DB-PASSWORD@aws-0-YOUR-REGION.pooler.supabase.com:5432/postgres
```

> Use the **Session pooler** (port `5432`): it's IPv4 (the direct `db.<ref>.supabase.co:5432` connection is IPv6-only on the free tier) and supports EF Core migrations (the "Transaction pooler" on `6543` does not).

The `.env` file is git-ignored and never committed.

### 3. Start the application

```bash
docker compose up --build
```

The database migrations are applied automatically on startup and the sample data is seeded.

The app is then available at:

```
http://localhost:8080
```

### 4. Stop the application

```bash
docker compose down
```

---

## 🚀 Deploying (Google Cloud VM + Supabase)

The database lives on Supabase (free, managed), so the VM only needs to run the lightweight ASP.NET Core app — a free `e2-micro` instance is plenty.

### 1. Create the VM

Google Cloud Console → Compute Engine → Create instance:

- **Name:** `wash-zone`
- **Machine type:** `e2-micro` (free tier)
- **Boot disk:** Ubuntu 22.04 LTS
- **Firewall:** allow TCP `8080`

Or via `gcloud`:

```bash
gcloud compute instances create wash-zone \
  --zone=us-central1-a \
  --machine-type=e2-micro \
  --image-family=ubuntu-2204-lts --image-project=ubuntu-os-cloud \
  --tags=washzone

gcloud compute firewall-rules create allow-washzone \
  --allow tcp:8080 --target-tags=washzone
```

### 2. Install Docker on the VM

```bash
gcloud compute ssh wash-zone --zone=us-central1-a
sudo apt update && sudo apt install -y docker.io docker-compose-plugin
sudo usermod -aG docker $USER
```

Log out and back in for the `docker` group to apply.

### 3. Deploy

```bash
git clone https://github.com/glafver/wash-zone.git
cd wash-zone
cp .env.example .env
# edit .env → set ConnectionStrings__DefaultConnection to your Supabase connection string
docker compose up -d --build
```

### 4. Open the app

```
http://<VM_EXTERNAL_IP>:8080
```

Get the public IP:

```bash
gcloud compute instances describe wash-zone --zone=us-central1-a \
  --format='get(networkInterfaces[0].accessConfigs[0].natIP)'
```

### Optional: HTTPS + custom domain

Add a Caddy reverse proxy to `docker-compose.yml`:

```yaml
  caddy:
    image: caddy:2
    ports:
      - "80:80"
      - "443:443"
    volumes:
      - ./Caddyfile:/etc/caddy/Caddyfile
    depends_on:
      - washzone
```

`Caddyfile`:

```Caddyfile
yourdomain.com {
    reverse_proxy washzone:8080
}
```

Then run `docker compose up -d` again.

---

## 💻 Local development (without Docker for the app)

If you prefer to run the app with `dotnet run`, point it at your Supabase database via a git-ignored `appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "postgresql://postgres.YOUR-PROJECT-REF:YOUR-DB-PASSWORD@aws-0-YOUR-REGION.pooler.supabase.com:5432/postgres"
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

The project uses EF Core **code-first migrations** (committed under `Migrations/`) with the **Npgsql** PostgreSQL provider. They are applied automatically at startup via `context.Database.Migrate()`.

To add a new migration, restore the local EF tool and run:

```bash
dotnet tool restore
dotnet tool run dotnet-ef migrations add <MigrationName>
```

> The EF tool is pinned to `8.0.11` in `.config/dotnet-tools.json` to match the EF Core version used by the project.
>
> A design-time factory (`Data/ApplicationDbContextFactory.cs`) lets `dotnet ef` build the context without running the app.

---

## 📁 Project structure

```
WashZone/
├── .github/workflows/ci.yml      # CI build pipeline
├── docker-compose.yml            # the app container
├── Dockerfile                    # multi-stage .NET build
├── .env.example                  # environment variable template
│
├── Program.cs                    # entry point (DI, Identity, migrations, seeding)
├── appsettings.json
├── WashZone.csproj / .sln
│
├── Data/
│   ├── ApplicationDbContext.cs         # DbContext + EF model configuration
│   ├── ApplicationDbContextFactory.cs  # design-time factory for `dotnet ef`
│   └── SampleData.cs                   # seed data
│
├── Models/                       # domain entities
│   ├── Station.cs
│   ├── Package.cs
│   ├── Feature.cs
│   ├── Booking.cs
│   ├── StationPackage.cs         # many-to-many join
│   ├── PackageFeature.cs         # many-to-many join
│   ├── StationAdmin.cs           # user → station assignment
│   └── UserVehicle.cs            # saved car registration numbers
│
├── Migrations/                   # EF Core migrations (PostgreSQL)
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
