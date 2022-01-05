namespace Consensus.ApiContracts
{
    public class AgentLog
    {
        public LogLevel Level { get; set; }
        public string Message { get; set; }
        public IEnumerable<object> Params { get; set; }
    }

    /// <summary>
    /// Serilog-compatible log level.
    /// 
    /// </summary>
    public enum LogLevel
    {
        Verbose,
        Debug,
        Information,
        Warning,
        Error,
        Fatal
    }
}
