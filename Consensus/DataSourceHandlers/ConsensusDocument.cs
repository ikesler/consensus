namespace Consensus.DataSourceHandlers
{
    public class ConsensusDocument
    {
        public string Source;
        public string ExternalSource;
        public DateTime CreatedAt;
        public DateTime? ExternalCreatedAt;
        public string ExternalId;
        public string Url;
        public string CreatedById;
        public string CreatedByName;
        public string[] AttachmentUrls;
        public string Content;
        public ConsensusDocument[] Children;
    }
}
