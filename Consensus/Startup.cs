using Consensus.DataSourceHandlers;
using Consensus.DataSourceHandlers.Vk;
using Consensus.Elastic;
using Consensus.Hangfire;
using DryIoc;
using Hangfire;
using Hangfire.PostgreSql;

namespace Consensus
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHangfireServer(new PostgreSqlStorage(Configuration.GetConnectionString("HangfireStorage")));
            services.AddHangfire(x => x.UsePostgreSqlStorage(Configuration.GetConnectionString("HangfireStorage")));
            services.AddControllers();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseHangfireDashboard();
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });

        }

        public void ConfigureContainer(IContainer container)
        {
            container.Register<ConsensusDocumentRepository>();
            container.Register<JobFilter>();
            container.RegisterInstance(Configuration.Get<SysConfig>());
            GlobalJobFilters.Filters.Add(container.Resolve<JobFilter>());
            GlobalConfiguration.Configuration.UseActivator(new ContainerJobActivator(container));
            container.Register(typeof(IDataSourceHandler), typeof(VkDataSourceHandler));
        }

        /// <summary>
        /// Use this method to pass your custom pre-configured container to the `IHostBuilder.UseServiceProviderFactory` in "Program.cs"
        /// </summary>
        public static IContainer CreateMyPreConfiguredContainer() =>
            // This is an example configuration,
            // for possible options check the https://github.com/dadhi/DryIoc/blob/master/docs/DryIoc.Docs/RulesAndDefaultConventions.md
            new Container(rules =>

                // Configures property injection for Controllers, ensure that you've added `AddControllersAsServices` in `ConfigureServices`
                rules.With(propertiesAndFields: request => 
                    request.ServiceType.Name.EndsWith("Controller") ? PropertiesAndFields.Properties()(request) : null)
            );
    }
}
