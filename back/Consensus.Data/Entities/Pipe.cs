namespace Consensus.Data.Entities
{
    public class Pipe
    {
        public long Id { get; set; }
        public Guid PublicId { get; set; }
        public PipeStatus Status { get; set; }
        public string DataSourceCode { get; set; }
        public string PropsJson { get; set; }
        public string? StateJson { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime OpenedAt { get; set; }
        public DateTime LastPumpedAt { get; set; }
    }
}
