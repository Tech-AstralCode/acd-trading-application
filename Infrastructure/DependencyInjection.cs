using Application.Abstractions;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        var cs = config.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Missing ConnectionStrings:DefaultConnection");
        services.AddDbContext<TradingDbContext>(opt =>
        {
            opt.UseNpgsql(cs, npg => npg.EnableRetryOnFailure());
        });

        services.AddScoped<ITradingRepository, TradingRepository>();
        // Serilog (wired in Program.cs)
        return services;
    }
}