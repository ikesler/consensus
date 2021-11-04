namespace Consensus.Common.Configuration
{
    public class SysConfig
    {
        public string BackEndUrl { get; set; }
        public string FrontEndUrl { get; set; }
        public string ElasticEndpoint { get; set; }
        public string[] KnownProxies { get; set; }

        public Dictionary<string, DataSourceConfig> ConsensusDataSources { get; set; }
    }
}
