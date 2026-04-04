using Microsoft.EntityFrameworkCore;
using SkyBot.Models;

namespace SkyBot.Data;

/// <summary>
/// The main EF Core database context for SkyBot.
/// Configures all tables, relationships, and indexes using Fluent API.
/// </summary>
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<BotUser> BotUsers { get; set; } = null!;
    public DbSet<SearchHistory> SearchHistories { get; set; } = null!;
    public DbSet<Subscription> Subscriptions { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ── BotUser configuration ────────────────────────────────────────────
        modelBuilder.Entity<BotUser>(entity =>
        {
            // TelegramUserId must be unique across all users
            entity.HasIndex(u => u.TelegramUserId).IsUnique();

            entity.Property(u => u.FirstName).IsRequired().HasMaxLength(64);
            entity.Property(u => u.Username).HasMaxLength(32);
            entity.Property(u => u.LastName).HasMaxLength(64);
            entity.Property(u => u.SubscribedCity).HasMaxLength(100);
        });

        // ── SearchHistory configuration ──────────────────────────────────────
        modelBuilder.Entity<SearchHistory>(entity =>
        {
            // Index on TelegramUserId for fast lookups per user
            entity.HasIndex(sh => sh.TelegramUserId);

            entity.Property(sh => sh.CityName).IsRequired().HasMaxLength(100);
            entity.Property(sh => sh.WeatherSummary).IsRequired().HasMaxLength(500);

            // Many SearchHistories → one BotUser
            entity.HasOne(sh => sh.BotUser)
                  .WithMany(u => u.SearchHistories)
                  .HasForeignKey(sh => sh.TelegramUserId)
                  .HasPrincipalKey(u => u.TelegramUserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // ── Subscription configuration ───────────────────────────────────────
        modelBuilder.Entity<Subscription>(entity =>
        {
            // Index on TelegramUserId for fast lookups
            entity.HasIndex(s => s.TelegramUserId);
            // Each user can have only one subscription
            entity.HasIndex(s => s.TelegramUserId).IsUnique();

            entity.Property(s => s.City).IsRequired().HasMaxLength(100);

            // One Subscription → one BotUser
            entity.HasOne(s => s.BotUser)
                  .WithOne(u => u.Subscription)
                  .HasForeignKey<Subscription>(s => s.TelegramUserId)
                  .HasPrincipalKey<BotUser>(u => u.TelegramUserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
