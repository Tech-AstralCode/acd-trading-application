using Infrastructure;
using Infrastructure.MarketData;
using Infrastructure.Persistence;
using Application.Services;
using Serilog;
using Domain.Strategies;
using Infrastructure.Strategies;
using Application.Strategies;
using Infrastructure.Workers;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration
  .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
  .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true)
  .AddJsonFile("appsettings.Development.local.json", optional: true, reloadOnChange: true)
  .AddEnvironmentVariables();

// Serilog
Log.Logger = new LoggerConfiguration()
  .Enrich.FromLogContext()
  .WriteTo.Console()
  .WriteTo.File("logs/web-.log", rollingInterval: RollingInterval.Day)
  .CreateLogger();

builder.Host.UseSerilog();

// Services
builder.Services.AddControllersWithViews();
builder.Services.AddMemoryCache();

builder.Services.AddInfrastructure(builder.Configuration);

// Market data mock + cache worker
builder.Services.AddSingleton<IChainSource, MockChainSource>();
builder.Services.AddHostedService<ChainCacheService>();
// Services for queries
builder.Services.AddSingleton<IChainQueryService, ChainQueryService>();
builder.Services.AddSingleton<ICandleQueryService, CandleQueryService>();
// Market data
builder.Services.AddSingleton<ICandleSource, MockCandleSource>();
builder.Services.AddHostedService<CandleCacheService>();
// Strategies
builder.Services.AddSingleton<IStrategy, CrossoverStrategy>();
builder.Services.AddSingleton<IStrategyRegistry, StrategyRegistry>();
// Runner + Worker
builder.Services.AddScoped<IStrategyRunner, StrategyRunner>();
builder.Services.AddHostedService<SignalWorker>();

// Read service for controllers
builder.Services.AddScoped<ISignalsReadService, SignalsReadService>();

var app = builder.Build();

// Initialize DB + seed
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TradingDbContext>();
    await DbInitializer.InitializeAsync(db);
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.MapControllerRoute(
name: "default",
pattern: "{controller=Dashboard}/{action=Index}/{id?}");

app.Run();
