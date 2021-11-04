using Consensus.Common.Configuration;
using Consensus.Quartz.Jobs;
using Microsoft.Extensions.Hosting;
using Quartz;

namespace Consensus.Quartz
{
    public class ConsensusJobSheduler: IHostedService
    {
        private readonly SysConfig _sysConfig;
        private readonly IScheduler _scheduler;

        public ConsensusJobSheduler(SysConfig sysConfig, IScheduler scheduler)
        {
            _sysConfig = sysConfig;
            _scheduler = scheduler;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            foreach (var dataSource in _sysConfig.ConsensusDataSources)
            {
                var job = JobBuilder.Create<PumpDataSourceJob>()
                    .WithIdentity(dataSource.Key, "PumpDataSource")
                    .Build();
                var trigger = TriggerBuilder.Create()
                    .WithIdentity(dataSource.Key, "PumpDataSource")
                    .StartNow()
                    .WithCronSchedule(dataSource.Value.Schedule)
                    .Build();

                await _scheduler.ScheduleJob(job, new[] { trigger }, true, cancellationToken);
            }

            await _scheduler.Start(cancellationToken);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _scheduler.Shutdown(cancellationToken);
        }
    }
}
