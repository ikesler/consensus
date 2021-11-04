using Newtonsoft.Json.Linq;

namespace Consensus.DataSourceHandlers.Api
{
    public abstract class DataSourceHandlerBase<TConfig, TProps, TState> : IDataSourceHandler<TConfig, TProps, TState>
    {
        Type IDataSourceHandler.TConfig => typeof(TConfig);
        Type IDataSourceHandler.TProps => typeof(TProps);
        Type IDataSourceHandler.TState => typeof(TState);

        public abstract string Code { get; }
        public abstract Uri InitCallback(TConfig config, TProps props, Uri callbackUrl);
        public abstract TState HandleCallback(TConfig config, TProps props, Uri callbackUrl);
        public abstract (ConsensusDocument[], TState) PumpDocuments(TConfig config, TProps props, TState state);

        public Uri InitCallback(object config, object props, Uri callbackUrl)
        {
            return InitCallback(Cast<TConfig>(config), Cast<TProps>(props), callbackUrl);
        }

        public object HandleCallback(object config, object props, Uri callbackUrl)
        {
            return HandleCallback(Cast<TConfig>(config), Cast<TProps>(props), callbackUrl);
        }

        public (ConsensusDocument[], object) PumpDocuments(object config, object props, object state)
        {
            return PumpDocuments(Cast<TConfig>(config), Cast<TProps>(props), Cast<TState>(state));
        }

        private T Cast<T>(object obj)
        {
            if (obj is JObject jobject)
            {
                return jobject.ToObject<T>();
            }
            return (T)obj;
        }
    }
}
