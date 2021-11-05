using Consensus.Common.Configuration;
using Consensus.Quartz.Jobs;
using Microsoft.Extensions.Hosting;
using Quartz;
using Serilog;

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

        public async Task StartAsync(CancellationToken hostedServiceToken)
        {
            Log.Information("Scheduling Quartz jobs");
            foreach (var dataSource in _sysConfig.ConsensusDataSources)
            {
                var timeoutTokenSource = new CancellationTokenSource(dataSource.Value.Timeout);
                var cancellationToken = CancellationTokenSource
                    .CreateLinkedTokenSource(timeoutTokenSource.Token, hostedServiceToken)
                    .Token;

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

            await _scheduler.Start(hostedServiceToken);

            Log.Information("Scheduled Quartz jobs");
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            Log.Information("Shutting down Quartz jobs");

            await _scheduler.Shutdown(cancellationToken);

            Log.Information("Shut down Quartz jobs");
        }
    }
}
