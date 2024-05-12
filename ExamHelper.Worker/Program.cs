using Serilog;
using Serilog.Events;
using ExamHelper.Worker;
var confgSetting = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json").Build();
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .WriteTo.File(confgSetting["Logging:LogPath"], rollingInterval: RollingInterval.Day)
    .CreateLogger();

try
{
    Log.Information("Starting host");
    CreateHostBuilder(args).Build().Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}

static IHostBuilder CreateHostBuilder(string[] args) =>
    Host.CreateDefaultBuilder(args)
        .UseWindowsService()
        .UseSerilog()
        .ConfigureServices((hostContext, services) =>
        {
            services.AddHostedService<Worker>();
        });