using Microsoft.EntityFrameworkCore;
using Quartz;
using SkyBot.Data;
using SkyBot.Handlers;
using SkyBot.Jobs;
using SkyBot.Services;
using SkyBot.Services.Interfaces;
using System.Text.Json;
using Telegram.Bot;

var builder = WebApplication.CreateBuilder(args);

// ═══════════════════════════════════════════════════════════════════════════
//  1. DATABASE
// ═══════════════════════════════════════════════════════════════════════════

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// ═══════════════════════════════════════════════════════════════════════════
//  2. TELEGRAM BOT CLIENT (singleton)
// ═══════════════════════════════════════════════════════════════════════════

builder.Services.AddSingleton<ITelegramBotClient>(_ =>
{
    var token = builder.Configuration["BotSettings:Token"]
        ?? throw new InvalidOperationException("BotSettings:Token is missing.");
    return new TelegramBotClient(token);
});

// ═══════════════════════════════════════════════════════════════════════════
//  3. NAMED HTTP CLIENT for OpenWeatherMap
// ═══════════════════════════════════════════════════════════════════════════

builder.Services.AddHttpClient("WeatherClient", client =>
{
    client.Timeout = TimeSpan.FromSeconds(10);
});

// ═══════════════════════════════════════════════════════════════════════════
//  4. APPLICATION SERVICES
// ═══════════════════════════════════════════════════════════════════════════

builder.Services.AddSingleton<UserStateService>();      // must outlive requests
builder.Services.AddSingleton<LocationService>();


builder.Services.AddScoped<IWeatherService, WeatherService>();
builder.Services.AddScoped<SubscriptionService>();

builder.Services.AddScoped<CommandHandler>();
builder.Services.AddScoped<CallbackQueryHandler>();
builder.Services.AddScoped<AdminHandler>();
builder.Services.AddScoped<UpdateHandler>();

// ═══════════════════════════════════════════════════════════════════════════
//  5. QUARTZ.NET — fires DailyWeatherJob at the top of every hour
// ═══════════════════════════════════════════════════════════════════════════

builder.Services.AddQuartz(q =>
{
    var jobKey = new JobKey("DailyWeatherJob");
    q.AddJob<DailyWeatherJob>(opts => opts.WithIdentity(jobKey));
    q.AddTrigger(opts => opts
        .ForJob(jobKey)
        .WithIdentity("DailyWeatherJob-trigger")
        .WithCronSchedule("0 0 * * * ?"));   // every hour at :00
});
builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

// ═══════════════════════════════════════════════════════════════════════════
//  6. CONTROLLERS + JSON
//     Telegram sends snake_case JSON → configure ASP.NET accordingly.
// ═══════════════════════════════════════════════════════════════════════════

builder.Services.AddControllers()
    .AddJsonOptions(opts =>
    {
        // snake_case is built-in in .NET 8 System.Text.Json
        opts.JsonSerializerOptions.PropertyNamingPolicy        = JsonNamingPolicy.SnakeCaseLower;
        opts.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ═══════════════════════════════════════════════════════════════════════════
//  BUILD
// ═══════════════════════════════════════════════════════════════════════════

var app = builder.Build();

// ═══════════════════════════════════════════════════════════════════════════
//  7. AUTO-APPLY MIGRATIONS
// ═══════════════════════════════════════════════════════════════════════════

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
    app.Logger.LogInformation("✅ Database migration applied.");
}

// ═══════════════════════════════════════════════════════════════════════════
//  8. REGISTER TELEGRAM WEBHOOK
// ═══════════════════════════════════════════════════════════════════════════

using (var scope = app.Services.CreateScope())
{
    var botClient = scope.ServiceProvider.GetRequiredService<ITelegramBotClient>();

    var webhookBase  = app.Configuration["BotSettings:WebhookUrl"]
        ?? throw new InvalidOperationException("BotSettings:WebhookUrl is missing.");
    var secretToken  = app.Configuration["BotSettings:SecretToken"]
        ?? throw new InvalidOperationException("BotSettings:SecretToken is missing.");

    var webhookUrl = $"{webhookBase.TrimEnd('/')}/{secretToken}";

    await botClient.SetWebhook(
        url: webhookUrl,
        secretToken: secretToken);

    app.Logger.LogInformation("✅ Webhook registered: {Url}", webhookUrl);
}

// ═══════════════════════════════════════════════════════════════════════════
//  9. MIDDLEWARE PIPELINE
// ═══════════════════════════════════════════════════════════════════════════

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.MapControllers();
app.Run();
