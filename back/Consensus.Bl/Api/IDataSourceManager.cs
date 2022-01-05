using Consensus.Data.Entities;
using Consensus.DataSourceHandlers.Api;

namespace Consensus.Bl.Api
{
    public interface IDataSourceManager
    {
        Task<Uri> InitCallback(string dataSourceCode, string propsJson);
        Task HandleCallback(Guid pipeId, Uri callbackUrl);
        Task PumpDocuments(string dataSourceCode);
        Task<IEnumerable<Pipe>> GetPipes(string[] dataSourceCodes);
        Task PumpDocumentsFromAgent(Guid pipeId, string newStateJson, ConsensusDocument[] documents);
    }
}
