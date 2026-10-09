# ---- Build stage ----
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Restore dependencies first (better layer caching)
COPY ["WashZone.csproj", "./"]
RUN dotnet restore "WashZone.csproj"

# Copy the rest of the source and publish
COPY . .
RUN dotnet publish "WashZone.csproj" -c Release -o /app/publish /p:UseAppHost=false

# ---- Runtime stage ----
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "WashZone.dll"]
