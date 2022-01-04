using System;
using System.Collections.Generic;

namespace Consensus.Agents.Viber.DAL
{
    public partial class EventsMetaDatum
    {
        public long EventId { get; set; }
        public long Type { get; set; }
        public string Value { get; set; }

        public virtual Event Event { get; set; }
    }
}
