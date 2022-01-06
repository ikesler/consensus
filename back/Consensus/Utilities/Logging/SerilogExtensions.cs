namespace Consensus.Utilities.Logging
{
    public static class SerilogExtensions
    {
        public static Serilog.ILogger ForContext(this Serilog.ILogger logger, IDictionary<string, object> context)
        {
            return logger.ForContext(new PropertyBagEnricher(context));
        }
    }
}
