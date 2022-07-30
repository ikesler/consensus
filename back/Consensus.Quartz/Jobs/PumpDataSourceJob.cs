using Consensus.Bl.Api;
using Quartz;
using Serilog;
using Serilog.Context;

namespace Consensus.Quartz.Jobs
{
    public class PumpDataSourceJob : IJob
    {
        private readonly IDataSourceManager _dataSourceManager;

        public PumpDataSourceJob(IDataSourceManager dataSourceManager)
        {
            _dataSourceManager = dataSourceManager;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            using (LogContext.PushProperty("Source", context.JobDetail.Key.Name))
            {
                try
                {
                    await _dataSourceManager.PumpDocuments(context.JobDetail.Key.Name);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "An error while pumping data from {Source}");
                }
            }
        }
    }
}
