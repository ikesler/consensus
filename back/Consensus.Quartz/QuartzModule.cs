using Autofac;
using Autofac.Extras.Quartz;
using System.Collections.Specialized;

namespace Consensus.Quartz
{
    public class QuartzModule : Module
    {
        private readonly string _connectionString;

        public QuartzModule(string connectionString)
        {
            _connectionString = connectionString;
        }

        protected override void Load(ContainerBuilder cb)
        {
            var schedulerConfig = new NameValueCollection
            {
                { "quartz.threadPool.threadCount", "3" },
                { "quartz.scheduler.threadName", "Scheduler" },
                { "quartz.jobStore.type", "Quartz.Impl.AdoJobStore.JobStoreTX, Quartz" },
                { "quartz.jobStore.driverDelegateType", "Quartz.Impl.AdoJobStore.PostgreSQLDelegate, Quartz" },
                { "quartz.jobStore.tablePrefix", "QRTZ_" },
                { "quartz.jobStore.dataSource", "myDS" },
                { "quartz.dataSource.myDS.connectionString", _connectionString },
                { "quartz.dataSource.myDS.provider", "Npgsql" },
                { "quartz.serializer.type", "json" }
            };

            cb.RegisterModule(new QuartzAutofacFactoryModule
            {
                ConfigurationProvider = _ => schedulerConfig,
            });
            cb.RegisterModule(new QuartzAutofacJobsModule(GetType().Assembly));
        }
    }
}
