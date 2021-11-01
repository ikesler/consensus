using Consensus.DataSourceHandlers;
using Nest;

namespace Consensus.Elastic
{
    public class ConsensusDocumentRepository
    {
        private readonly SysConfig _config;

        public ConsensusDocumentRepository(SysConfig config)
        {
            _config = config;
        }

        public void Save(ConsensusDocument[] documents)
        {
            var node = new Uri(_config.ElasticEndpoint);
            var settings = new ConnectionSettings(node);
            var client = new ElasticClient(settings);
            var groupsByIndex = documents.GroupBy(d => d.Source);
            foreach (var group in groupsByIndex)
            {
                var r = client.Bulk(b => b.Index($"consensus_{group.Key}_{DateTime.Now.Date:yyyy.MM.dd}".ToLower()).IndexMany(group));
            }
        }
    }
}
