#!/bin/bash

# SkyBot Telegram Bot - Start Script

echo "🤖 Starting SkyBot..."

# Navigate to the SkyBot directory
cd "$(dirname "$0")/SkyBot" || exit 1

# Check if .NET SDK is installed
if ! command -v dotnet &> /dev/null; then
    echo "❌ Error: .NET SDK is not installed or not in PATH"
    echo "   Download from: https://dotnet.microsoft.com/download"
    exit 1
fi

# Check if appsettings.json exists
if [ ! -f "appsettings.json" ]; then
    echo "❌ Error: appsettings.json not found in SkyBot directory"
    exit 1
fi

# Restore dependencies
echo "📦 Restoring dependencies..."
dotnet restore

# Build the project
echo "🔨 Building project..."
dotnet build --no-restore

# Run the application
echo "🚀 Starting SkyBot..."
echo "   Press Ctrl+C to stop the bot"
echo ""
dotnet run
