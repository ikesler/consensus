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

Serilog.Debugging.SelfLog.Enable(msg => Console.WriteLine(msg));

var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
var builder = new ConfigurationBuilder()
    .AddJsonFile($"appsettings.json", true, true)
    .AddJsonFile($"appsettings.{env}.json", true, true)
    .AddEnvironmentVariables();

var config = builder.Build();

// Restrict app to single instance
using (var mutex = new Mutex(false, config.GetValue<string>("AppId")))
{
    if (!mutex.WaitOne(0, false))
    {
        Console.WriteLine("Another instance is running.");
        return;
    }

    if (config.GetValue<bool>("StartOnBoot"))
    {
        using var key = Registry.CurrentUser.CreateSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
        key.SetValue("Consensus.Agent", "\"" + System.Reflection.Assembly.GetExecutingAssembly().Location + "\"");
    }

    Console.WriteLine("Application Starting.");

    await Host.CreateDefaultBuilder(args)
        .UseSerilog((context, services, configuration) => configuration.ReadFrom.Configuration(config))
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
