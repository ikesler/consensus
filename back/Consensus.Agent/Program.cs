using Consensus.Agent;
using Consensus.ApiContracts;
using Consensus.DataSourceHandlers.Api;
using Consensus.DataSourceHandlers.Viber;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging.EventLog;
using Refit;

var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
var builder = new ConfigurationBuilder()
    .AddJsonFile($"appsettings.json", true, true)
    .AddJsonFile($"appsettings.{env}.json", true, true)
    .AddEnvironmentVariables();

var config = builder.Build();

await Host.CreateDefaultBuilder(args)
    .ConfigureLogging(configureLogging => { })
    .ConfigureServices((hostContext, services) =>
    {
        services.AddSingleton<IAgentApi>(x => RestService.For<IAgentApi>(config.GetValue<string>("ApiUrl")));
        services.AddTransient<ViberDataSourceHandler>();
        services.AddTransient<IDataSourceHandler>(x => x.GetService<ViberDataSourceHandler>());
        services.AddHostedService<PumpWorker>();
    })
    .UseWindowsService()
    .Build()
    .RunAsync();
