using Microsoft.Extensions.Configuration;

namespace Consensus.Common.Configuration
{
    public class DataSourceConfig
    {
        public string Schedule { get; set; }
        public TimeSpan Timeout { get; set; }
        public IConfigurationSection Config { get; set; }
    }
}
