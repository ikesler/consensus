using Autofac;
using Consensus.Bl;
using Consensus.Common;
using Consensus.Common.Configuration;
using Consensus.Data;
using Consensus.Quartz;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Text.Json;
using System.Text.Json.Serialization;

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
            var sysConfig = Configuration.Get<SysConfig>();
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.All;
                options.KnownNetworks.Clear();

                options.KnownProxies.Clear();
                if (sysConfig.KnownProxies != null)
                {
                    foreach (var knownProxy in sysConfig.KnownProxies)
                    {
                        options.KnownProxies.Add(System.Net.IPAddress.Parse(knownProxy));
                    }
                }

                options.AllowedHosts.Clear();
                var backEndUrl = new Uri(sysConfig.BackEndUrl);
                options.AllowedHosts.Add(backEndUrl.Host);
            });

            services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
            });
            services.AddDbContext<ConsensusDbContext>(o => o.UseNpgsql(Configuration.GetConnectionString("ConsensusDb")));

            services.AddHostedService<ConsensusDbMigrator>();
            services.AddHostedService<ConsensusJobSheduler>();
            services.AddAutoMapper(typeof(Startup));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseForwardedHeaders();
            app.UseSerilogRequestLogging();

            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        public void ConfigureContainer(ContainerBuilder cb)
        {
            cb.RegisterModule(new CommonModule(Configuration));
            cb.RegisterModule<BlModule>();
            cb.RegisterModule(new QuartzModule(Configuration.GetConnectionString("ConsensusDb")));
        }
    }
}
