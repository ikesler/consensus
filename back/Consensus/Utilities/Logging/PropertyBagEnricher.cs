using Serilog.Core;
using Serilog.Events;

namespace Consensus.Utilities.Logging
{
    /// <summary>
    /// Based on https://benfoster.io/blog/serilog-best-practices/#property-bag-enricher
    /// </summary>
    public class PropertyBagEnricher : ILogEventEnricher
    {
        private readonly IDictionary<string, object> _properties;

        public PropertyBagEnricher(IDictionary<string, object> properties)
        {
            _properties = properties;
        }

        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            foreach (var kvp in _properties)
            {
                logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty(kvp.Key, kvp.Value));
            }
        }
    }
}
