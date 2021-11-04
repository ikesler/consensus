using Autofac.Extensions.DependencyInjection;
using Consensus;
using Serilog;

Serilog.Debugging.SelfLog.Enable(Console.Error);

Host.CreateDefaultBuilder(args)
     .UseServiceProviderFactory(new AutofacServiceProviderFactory())
     .UseSerilog((context, services, configuration) => configuration.ReadFrom.Configuration(context.Configuration))
    .ConfigureWebHostDefaults(webBuilder =>
    {
        webBuilder.UseStartup<Startup>();
    })
    .Build()
    .Run();
