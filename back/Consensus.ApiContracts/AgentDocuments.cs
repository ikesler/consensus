using Consensus.DataSourceHandlers.Api;

namespace Consensus.ApiContracts
{
    public class AgentDocuments
    {
        public Guid PipeId { get; set; }
        public ConsensusDocument[] Documents { get; set; }
        public string StateJson { get; set; }
    }
}
