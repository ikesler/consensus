namespace Consensus.DataSourceHandlers.Api
{
    public interface IDataSourceHandler
    {
        string Code { get; }
        Type TConfig { get; }
        Type TProps { get; }
        Type TState { get; }
        Task<Uri> InitCallback(object config, object props, Uri callbackUrl);
        Task<object> HandleCallback(object config, object props, Uri callbackUrl);
        Task<(ConsensusDocument[], object)> PumpDocuments(object config, object props, object state);
    }

    public interface IDataSourceHandler<TConfig, TProps, TState> : IDataSourceHandler
    {
        Task<Uri> InitCallback(TConfig config, TProps props, Uri callbackUrl);
        Task<TState> HandleCallback(TConfig config, TProps props, Uri callbackUrl);
        Task<(ConsensusDocument[], TState)> PumpDocuments(TConfig config, TProps props, TState state);
    }
}