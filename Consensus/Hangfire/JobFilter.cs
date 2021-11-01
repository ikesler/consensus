using Consensus.DataSourceHandlers;
using Consensus.Elastic;
using Hangfire.Client;
using Hangfire.Common;
using Hangfire.Server;
using Hangfire.States;
using Hangfire.Storage;
using Newtonsoft.Json;

namespace Consensus.Hangfire
{
    public class JobFilter : JobFilterAttribute, IClientFilter, IServerFilter, IElectStateFilter, IApplyStateFilter
    {
        private readonly ConsensusDocumentRepository _elastic;

        public JobFilter(ConsensusDocumentRepository elastic)
        {
            _elastic = elastic;
        }

        public void OnCreated(CreatedContext filterContext)
        {
            
        }

        public void OnCreating(CreatingContext filterContext)
        {
            filterContext.SetJobParameter("RecurringJodId", filterContext.Parameters["RecurringJobId"]);
        }

        public void OnPerformed(PerformedContext filterContext)
        {
            if (filterContext.Result != null)
            {
                var result = (ValueTuple<ConsensusDocument[], object>)filterContext.Result;
                _elastic.Save(result.Item1);
                var parentId = filterContext.GetJobParameter<string>("RecurringJobId");
                filterContext.Storage.GetConnection().SetRangeInHash($"{parentId}_state", new[]
                {
                    new KeyValuePair<string, string>("state", JsonConvert.SerializeObject(result.Item2))
                });
            }
        }

        public void OnPerforming(PerformingContext filterContext)
        {
            var parentId = filterContext.GetJobParameter<string>("RecurringJobId");
            string prevStateJson = null;
            filterContext.Storage.GetConnection().GetAllEntriesFromHash($"{parentId}_state")?.TryGetValue("state", out prevStateJson);
            
            if (prevStateJson != null)
            {
                var prevState = JsonConvert.DeserializeObject(prevStateJson);
                var mutableArgs = filterContext.BackgroundJob.Job.Args as object[];
                if (mutableArgs != null)
                {
                    mutableArgs[mutableArgs.Length - 1] = prevState;
                }
            }
        }

        public void OnStateApplied(ApplyStateContext context, IWriteOnlyTransaction transaction)
        {
            
        }

        public void OnStateElection(ElectStateContext context)
        {
            
        }

        public void OnStateUnapplied(ApplyStateContext context, IWriteOnlyTransaction transaction)
        {
            
        }
    }
}
