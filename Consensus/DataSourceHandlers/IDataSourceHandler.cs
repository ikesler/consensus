namespace Consensus.DataSourceHandlers
{
    public interface IDataSourceHandler
    {
        string Code { get; }
        Type TConfig { get; }
        Type TProps { get; }
        Type TState { get; }
        (object, object) HandleCallback(object config, Uri redirectUrl);
        object InitCallback(object config, object props);
        (ConsensusDocument[], object) RunJob(object config, object props, object state);
    }

    public interface IDataSourceHandler<TConfig, TProps, TState>: IDataSourceHandler
    {
        Uri InitCallback(TConfig config, TProps props);
        (TProps, TState) HandleCallback(TConfig config, Uri redirectUrl);
        (ConsensusDocument[], TState) RunJob(TConfig config, TProps props, TState state);
    }
}
