namespace Consensus.DataSourceHandlers
{
    public class ConsensusDocument
    {
        public string Source;
        public string SubSource;
        public DateTime CreatedAt;
        public DateTime? ExternalCreatedAt;
        public string ExternalId;
        public string Url;
        public string CreatedById;
        public string CreatedByName;
        public string[] AttachmentUrls;
        public string Content;
    }
}
