// Infrastructure/Workers/SignalWorker.cs
using Application.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Workers;

public sealed class SignalWorker : BackgroundService
{
    private readonly IServiceProvider _sp;
    private readonly string[] _underlyings;
    private readonly TimeSpan _every;

    public SignalWorker(IServiceProvider sp, IConfiguration cfg)
    {
        _sp = sp;
        _underlyings = cfg.GetSection("Trading:Underlyings").Get<string[]>() ?? new[] { "NIFTY" };
        _every = TimeSpan.FromSeconds(cfg.GetValue("Signals:EverySeconds", 5));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            foreach (var u in _underlyings)
            {
                try
                {
                    using var scope = _sp.CreateScope();
                    var runner = scope.ServiceProvider.GetRequiredService<IStrategyRunner>();
                    await runner.RunOnceAsync(u, stoppingToken);
                }
                catch (Exception)
                {
                    // log with Serilog if desired
                }
            }
            await Task.Delay(_every, stoppingToken);
        }
    }
}
