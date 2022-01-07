using Consensus.Agent;
using Consensus.ApiContracts;
using Consensus.DataSourceHandlers.Api;
using Consensus.DataSourceHandlers.Viber;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Win32;
using Refit;
using Serilog;
using Serilog.Events;
using System.Diagnostics;

var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
var builder = new ConfigurationBuilder()
    .AddJsonFile($"appsettings.json", true, true)
    .AddJsonFile($"appsettings.{env}.json", true, true)
    .AddEnvironmentVariables();

var config = builder.Build();

// Apparently, EventLog sink configuration from json file does not work properly
// But the coded one does. And we absolutely need Even log for a Windows app.
// https://github.com/serilog/serilog-sinks-eventlog/issues/36
Serilog.Debugging.SelfLog.Enable(msg => Debug.WriteLine(msg));
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .WriteTo.Debug()
    .WriteTo.EventLog("Application")
    .WriteTo.Http($"{config.GetValue<string>("ApiUrl")}/agent/logs")
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Application", "ConsensusAgent")
    .CreateLogger();

Log.Information("Starting application");

// Restrict app to single instance
using (var mutex = new Mutex(false, config.GetValue<string>("AppId")))
{
    if (!mutex.WaitOne(0, false))
    {
        return;
    }

    if (config.GetValue<bool>("StartOnBoot"))
    {
        using var key = Registry.CurrentUser.CreateSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
        key.SetValue("Consensus.Agent", "\"" + System.Reflection.Assembly.GetExecutingAssembly().Location + "\"");
    }

    await Host.CreateDefaultBuilder(args)
        .ConfigureServices((hostContext, services) =>
        {
            services.AddSingleton<IAgentApi>(x => RestService.For<IAgentApi>(config.GetValue<string>("ApiUrl")));
            services.AddSingleton<PureManClickOnce>(x => new PureManClickOnce(config.GetValue<string>("PublishUrl")));
            services.AddTransient<ViberDataSourceHandler>();
            services.AddTransient<IDataSourceHandler>(x => x.GetService<ViberDataSourceHandler>());
            services.AddHostedService<PumpWorker>();
        })
        .Build()
        .RunAsync();
}
