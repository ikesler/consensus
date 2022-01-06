namespace Consensus.Models
{
    public class AgentLogEvent
    {
        public Serilog.Events.LogEventLevel Level { get; set; }
        public DateTimeOffset Timestamp { get; set; }
        public string MessageTemplate { get; set; }
        public Dictionary<string, object> Properties { get; set; }

        public string Exception { get; set; }
    }
}
