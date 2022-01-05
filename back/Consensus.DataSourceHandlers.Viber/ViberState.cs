namespace Consensus.DataSourceHandlers.Viber
{
    public class ViberState
    {
        public DateTimeOffset? LastMessageDate { get; set; }
        public DateTimeOffset? FirstMessageDate { get; set; }
    }
}
