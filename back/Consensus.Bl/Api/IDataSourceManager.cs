namespace Consensus.Bl.Api
{
    public interface IDataSourceManager
    {
        Task<Uri> InitCallback(string dataSourceCode, string propsJson);
        Task HandleCallback(Guid pipeId, Uri callbackUrl);
        Task PumpDocuments(string dataSourceCode);
    }
}
