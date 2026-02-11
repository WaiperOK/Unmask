#!/usr/bin/env bash
set -euo pipefail

cd "$(dirname "$0")"

echo "[1/2] Building GUI..."
dotnet build Unmask.csproj --configuration Release

echo "[2/2] Building Console..."
dotnet build Unmask.Console.csproj --configuration Release

echo "Build completed:"
echo "- GUI:     bin/Release/net472/Unmask.exe"
echo "- Console: bin/Release/net472/Unmask.Console.exe"
