#!/bin/bash

if [ -z "$1" ]; then
  echo "Usage: $0 <MigrationName>"
  exit 1
fi

echo "Installing dotnet-ef if missing..."
dotnet tool install --global dotnet-ef || true

echo "Adding migration: $1"
dotnet ef migrations add "$1" --project ./TimeCapsule.csproj

echo "Updating database..."
dotnet ef database update --project ./TimeCapsule.csproj