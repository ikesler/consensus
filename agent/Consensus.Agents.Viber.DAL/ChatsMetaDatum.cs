using System;
using System.Collections.Generic;

namespace Consensus.Agents.Viber.DAL
{
    public partial class ChatsMetaDatum
    {
        public string ChatToken { get; set; }
        public long Type { get; set; }
        public string Value { get; set; }
    }
}
