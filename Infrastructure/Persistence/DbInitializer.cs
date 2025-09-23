using Domain.Config;
using Domain.Strategies;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public static class DbInitializer
{
    public static async Task InitializeAsync(TradingDbContext db)
    {
        await db.Database.MigrateAsync();

        if (!await db.RiskProfiles.AnyAsync())
        {
            db.RiskProfiles.Add(new RiskProfile
            {
                Id = Guid.NewGuid(),
                Name = "Conservative",
                PerTradeRiskPct = 0.75m,
                DailyLossCapR = 2m,
                MaxConcurrent = 1,
                SlippageBps = 8,
                NoEntryAfter = new TimeOnly(15, 10)
            });
        }

        if (!await db.Strategies.AnyAsync())
        {
            db.Strategies.Add(new Strategy
            {
                Id = Guid.NewGuid(),
                Name = "Crossover",
                Enabled = true,
                Mode = Domain.Strategies.StrategyMode.Manual,
                AiThreshold = 0.62m,
                ConfigJson = "{\"FastEMA\":\"9\",\"SlowEMA\":\"21\",\"MinMomentum\":\"0.25\",\"StrikeBias\":\"ATM\"}"
            });
        }

        await db.SaveChangesAsync();
    }
}