using Application.Abstractions;
using Domain.Config;
using Domain.Signals;
using Domain.Strategies;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public sealed class TradingDbContext : DbContext
{
    public TradingDbContext(DbContextOptions<TradingDbContext> options) : base(options) { }

    public DbSet<RiskProfile> RiskProfiles => Set<RiskProfile>();
    public DbSet<Strategy> Strategies => Set<Strategy>();
    public DbSet<Signal> Signals => Set<Signal>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.HasPostgresExtension("uuid-ossp");

        b.Entity<RiskProfile>(e =>
        {
            e.ToTable("risk_profiles");
            e.HasKey(x => x.Id);
        });

        b.Entity<Strategy>(e =>
        {
            e.ToTable("strategies");
            e.HasKey(x => x.Id);
            e.Property(x => x.Mode).HasConversion<int>();
            e.Property(x => x.ConfigJson).HasColumnType("jsonb");
        });

        b.Entity<Signal>(e =>
        {
            e.ToTable("signals");
            e.HasKey(x => x.Id);
            e.Property(x => x.Side).HasConversion<int>();
            e.Property(x => x.Expiry).HasColumnType("date");
            e.Property(x => x.ReasonJson).HasColumnType("jsonb");
        });
    }
}