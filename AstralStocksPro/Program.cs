using Infrastructure;
using Infrastructure.MarketData;
using Infrastructure.Persistence;
using Application.Services;
using Serilog;

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

// Application services
builder.Services.AddSingleton<IChainQueryService, ChainQueryService>();

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
