# SkyBot - Telegram Weather Bot

Telegram bot that provides weather information for cities in Uzbekistan. Supports 3 languages: Uzbek, English, and Russian.

## Features

- 🌤 Current weather for any city in Uzbekistan
- 📅 3-day weather forecast
- 🔔 Daily weather reminders at a scheduled time
- 🗺 Location selection via inline buttons (Region → District)
- 🌐 Multi-language support (Uzbek, English, Russian)
- ⚙️ Settings panel for language changes
- 📊 User statistics and search history
- 🛠 Admin panel for user management

## Tech Stack

- **Framework**: .NET 8.0 (ASP.NET Core)
- **Database**: SQLite (EF Core)
- **Telegram Bot Library**: Telegram.Bot v22.4.0
- **Scheduler**: Quartz.NET (hourly jobs)
- **Weather API**: OpenWeatherMap

## Prerequisites

- .NET 8.0 SDK or later
- SQLite (included)
- Telegram Bot Token (from [@BotFather](https://t.me/BotFather))
- OpenWeatherMap API Key (from [openweathermap.org](https://openweathermap.org/api))
- A server with a public URL (or ngrok for development)

## Installation

### 1. Clone the repository

```bash
git clone <your-repo-url>
cd SkyBot
```

### 2. Configure settings

Edit `appsettings.json`:

```json
{
  "BotSettings": {
    "Token": "YOUR_BOT_TOKEN",
    "WebhookUrl": "https://your-domain.com/bot/",
    "SecretToken": "your-secret-token",
    "AdminTelegramId": 123456789
  },
  "WeatherSettings": {
    "ApiKey": "YOUR_OPENWEATHERMAP_API_KEY",
    "BaseUrl": "https://api.openweathermap.org/data/2.5/"
  },
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=skybot.db"
  }
}
```

### 3. Get your credentials

1. **Telegram Bot Token**: Message [@BotFather](https://t.me/BotFather) on Telegram, create a new bot with `/newbot`, and get the token.
2. **OpenWeatherMap API Key**: Register at [openweathermap.org](https://openweathermap.org/api) and get a free API key.
3. **AdminTelegramId**: Your personal Telegram user ID (you can get it from [@userinfobot](https://t.me/userinfobot)).
4. **SecretToken**: Generate a random UUID (e.g., `uuidgen` on Linux or use an online generator).

### 4. Build and run

```bash
dotnet restore
dotnet build
dotnet run
```

The bot will:
1. Apply database migrations automatically
2. Set the webhook with Telegram
3. Start listening for updates

## Deployment

### Option 1: Docker (Recommended)

Create a `Dockerfile`:

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore
RUN dotnet build -c Release -o /app/build

FROM build AS publish
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SkyBot.dll"]
```

Build and run:

```bash
docker build -t skybot .
docker run -d --name skybot -p 5000:80 -v $(pwd)/data:/app skybot
```

### Option 2: Linux Systemd Service

1. Publish the app:

```bash
dotnet publish -c Release -o /opt/skybot
```

2. Create a systemd service file `/etc/systemd/system/skybot.service`:

```ini
[Unit]
Description=SkyBot Telegram Weather Bot
After=network.target

[Service]
Type=simple
User=skybot
WorkingDirectory=/opt/skybot
ExecStart=/usr/bin/dotnet /opt/skybot/SkyBot.dll
Restart=always
RestartSec=10

[Install]
WantedBy=multi-user.target
```

3. Enable and start:

```bash
sudo systemctl enable skybot
sudo systemctl start skybot
sudo systemctl status skybot
```

### Option 3: Windows Service

Use NSSM (Non-Sucking Service Manager):

```powershell
# Download NSSM from https://nssm.cc/
nssm install SkyBot "C:\Program Files\dotnet\dotnet.exe" "C:\path\to\SkyBot\SkyBot.dll"
nssm start SkyBot
```

### Option 4: Cloud Hosting

Deploy to any cloud provider that supports .NET:

- **Azure App Service**: Deploy via GitHub Actions or ZIP deploy
- **AWS Elastic Beanstalk**: Upload published ZIP
- **DigitalOcean App Platform**: Connect GitHub repo
- **Railway.app**: Connect repo, auto-detects .NET

## Database

The bot uses SQLite by default. The database file (`skybot.db`) is created automatically on first run.

### Manual migrations

```bash
dotnet ef migrations add <MigrationName>
dotnet ef database update
```

## Backup

To backup your database:

```bash
cp skybot.db skybot_backup.db
cp skybot.db-shm skybot_backup.db-shm
cp skybot.db-wal skybot_backup.db-wal
```

## Troubleshooting

### Bot doesn't respond

1. Check if the webhook is set correctly:
   ```bash
   curl https://api.telegram.org/bot<YOUR_TOKEN>/getWebhookInfo
   ```
2. Check logs for errors
3. Ensure the server is accessible from the internet

### Webhook errors

- Make sure your server is running on port 80 or 443
- Use HTTPS in production (Telegram requires HTTPS for webhooks)
- For development, use ngrok:
  ```bash
  ngrok http 5000
  ```
  Then update `WebhookUrl` in `appsettings.json`

### Database locked errors

Stop the bot before running migrations:

```bash
sudo systemctl stop skybot
dotnet ef database update
sudo systemctl start skybot
```

## API Endpoints

| Endpoint | Description |
|----------|-------------|
| `POST /bot/<secret-token>` | Telegram webhook endpoint |
| `GET /swagger` | Swagger UI (development only) |
| `GET /health` | Health check |

## Project Structure

```
SkyBot/
├── Controllers/       # Webhook controller
├── Data/              # EF Core DbContext, JSON data
│   ├── regions.json   # 14 regions of Uzbekistan
│   ├── districts.json # Districts by region
│   └── villages.json  # Villages (optional)
├── DTOs/              # Weather API DTOs
├── Handlers/          # Message handlers
├── Jobs/              # Quartz.NET scheduled jobs
├── Keyboards/         # Inline keyboard builders
├── Localization/      # Multi-language messages
│   ├── UzMessages.cs  # Uzbek
│   ├── EnMessages.cs  # English
│   ├── RuMessages.cs  # Russian
│   └── MessageResolver.cs
├── Models/            # Database models
├── Services/          # Business logic
└── Program.cs         # Entry point
```

## License

MIT License

## Author

SkyBot Development Team
