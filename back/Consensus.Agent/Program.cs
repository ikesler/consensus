using Consensus.Agent;
using Consensus.ApiContracts;
using Consensus.DataSourceHandlers.Api;
using Consensus.DataSourceHandlers.Viber;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Refit;
using Serilog;
using Serilog.Events;
using System.Diagnostics;
using System.Reflection;

var configStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Consensus.Agent.appsettings.json");
var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
var config = new ConfigurationBuilder()
    .AddJsonStream(configStream)
    .AddJsonFile($"appsettings.{env}.json", true)
    .AddEnvironmentVariables()
    .Build();

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

var agentApi = RestService.For<IAgentApi>(config.GetValue<string>("ApiUrl"));
var deployment = new Deployment(config, agentApi);

if (!deployment.IsDeployed)
{
    await deployment.DeployAndStart();
    return;
}

if (await deployment.CompleteUpdate())
{
    return;
}

// Restrict app to single instance
using (var mutex = new Mutex(false, config.GetValue<string>("AppId")))
{
    // non-zero timeout to wait untill deployment process has exited
    if (!mutex.WaitOne(5000, false))
    {
        return;
    }

    Log.Information("Running worker");
    await Host.CreateDefaultBuilder(args)
        .ConfigureServices((hostContext, services) =>
        {
            services.AddSingleton(deployment);
            services.AddSingleton(agentApi);
            services.AddTransient<ViberDataSourceHandler>();
            services.AddTransient<IDataSourceHandler>(x => x.GetService<ViberDataSourceHandler>());
            services.AddHostedService<PumpWorker>();
        })
        .Build()
        .RunAsync();
}
