using System;
using System.Collections.Generic;

namespace Consensus.Agents.Viber.DAL
{
    public partial class Call
    {
        public long EventId { get; set; }
        public long Type { get; set; }
        public long? Duration { get; set; }
        public long Status { get; set; }

        public virtual Event Event { get; set; }
    }
}
