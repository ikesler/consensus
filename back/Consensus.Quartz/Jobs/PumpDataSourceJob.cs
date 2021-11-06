using Consensus.Bl.Api;
using Quartz;

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
            await _dataSourceManager.PumpDocuments(context.JobDetail.Key.Name);
        }
    }
}
