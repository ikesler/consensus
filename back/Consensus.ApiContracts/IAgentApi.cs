using Refit;

namespace Consensus.ApiContracts
{
    public interface IAgentApi
    {
        [Get("/agent/pipes")]
        Task<Pipe[]> GetPipes([Query(CollectionFormat.Multi)] string[] sources, CancellationToken cancellationToken);

        [Post("/agent/documents")]
        Task PostDocuments(AgentDocuments agentDocuments, CancellationToken cancellationToken);

        [Post("/agent/logs")]
        Task PostLogs(AgentLog agentLog, CancellationToken cancellationToken);
    }
}
