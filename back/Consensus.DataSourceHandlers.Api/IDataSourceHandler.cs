namespace Consensus.DataSourceHandlers.Api
{
    public interface IDataSourceHandler
    {
        string Code { get; }
        Type TConfig { get; }
        Type TProps { get; }
        Type TState { get; }
        Uri InitCallback(object config, object props, Uri callbackUrl);
        object HandleCallback(object config, object props, Uri callbackUrl);
        (ConsensusDocument[], object) PumpDocuments(object config, object props, object state);
    }

    public interface IDataSourceHandler<TConfig, TProps, TState> : IDataSourceHandler
    {
        Uri InitCallback(TConfig config, TProps props, Uri callbackUrl);
        TState HandleCallback(TConfig config, TProps props, Uri callbackUrl);
        (ConsensusDocument[], TState) PumpDocuments(TConfig config, TProps props, TState state);
    }
}