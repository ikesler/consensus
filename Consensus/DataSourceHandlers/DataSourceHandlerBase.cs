namespace Consensus.DataSourceHandlers
{
    public abstract class DataSourceHandlerBase<TConfig, TProps, TState>: IDataSourceHandler<TConfig, TProps, TState>
    {
        protected readonly SysConfig SysConfig;

        protected DataSourceHandlerBase(SysConfig sysConfig)
        {
            SysConfig = sysConfig;
        }

        public abstract string Code { get; }
        public abstract Uri InitCallback(TConfig config, TProps props);
        public abstract ConsensusDocument[] RunJob(TConfig config, TProps props, TState state);
        public abstract (TProps, TState) HandleCallback(TConfig config, Uri redirectUrl);

        public (object, object) HandleCallback(object config, Uri redirectUrl)
        {
            return HandleCallback((TConfig) config, redirectUrl);
        }

        public object InitCallback(object config, object props)
        {
            return InitCallback((TConfig) config, (TProps) props);
        }

        public ConsensusDocument[] RunJob(object config, object props, object state)
        {
            return RunJob((TConfig) config, (TProps) props, (TState) state);
        }

        Type IDataSourceHandler.TConfig => typeof(TConfig);

        Type IDataSourceHandler.TProps => typeof(TProps);

        Type IDataSourceHandler.TState => typeof(TState);
    }
}
