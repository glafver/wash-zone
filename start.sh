#!/bin/bash

echo "Starting SQL Server..."

export ACCEPT_EULA=Y

/opt/mssql/bin/sqlservr &

echo "Waiting for SQL Server..."

sleep 40

echo "Starting WashZone..."

export ASPNETCORE_URLS=http://+:8080

dotnet /app/publish/WashZone.dll