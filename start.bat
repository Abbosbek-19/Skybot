@echo off
echo Starting SkyBot...
echo.

cd /d "%~dp0"

dotnet restore
dotnet build --no-restore
echo.
echo Starting SkyBot... Press Ctrl+C to stop.
echo.
dotnet run
