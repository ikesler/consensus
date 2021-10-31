using Consensus;
using DryIoc;
using DryIoc.Microsoft.DependencyInjection;

Host.CreateDefaultBuilder(args)
    .UseServiceProviderFactory(new DryIocServiceProviderFactory(Startup.CreateMyPreConfiguredContainer()))
    .ConfigureWebHostDefaults(webBuilder =>
    {
        webBuilder.UseStartup<Startup>();
    })
    .Build()
    .Run();
