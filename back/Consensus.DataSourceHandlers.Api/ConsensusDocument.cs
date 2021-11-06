namespace Consensus.DataSourceHandlers.Api
{
    public class ConsensusDocument
    {
        public Guid Id { get; set; }
        public string Source { get; set; }
        public string ExternalSource { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ExternalCreatedAt { get; set; }
        public string ExternalId { get; set; }
        public string Url { get; set; }
        public string? CreatedById { get; set; }
        public string? CreatedByName { get; set; }
        public string[] AttachmentUrls { get; set; } = new string[0];
        public string Content { get; set; }
        public Guid? ParentId { get; set; }
    }
}
