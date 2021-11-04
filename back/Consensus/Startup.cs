using Autofac;
using Consensus.Bl;
using Consensus.Common;
using Consensus.Data;
using Consensus.Quartz;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Serilog;

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
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.All;
                options.KnownNetworks.Clear();
                options.KnownProxies.Clear();
                options.AllowedHosts.Clear();
            });

            services.AddControllers();
            services.AddDbContext<ConsensusDbContext>(o => o.UseNpgsql(Configuration.GetConnectionString("ConsensusDb")));

            services.AddHostedService<ConsensusDbMigrator>();
            services.AddHostedService<ConsensusJobSheduler>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseForwardedHeaders();
            app.UseSerilogRequestLogging(opts =>
            {
                opts.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
                {
                    var request = httpContext.Request;
                    diagnosticContext.Set("RequestHeaders",request.Headers.Select(kvp => $"{kvp.Key}: {kvp.Value}"), true);
                };
            });

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
