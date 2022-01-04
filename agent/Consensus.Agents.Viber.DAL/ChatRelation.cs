using System;
using System.Collections.Generic;

namespace Consensus.Agents.Viber.DAL
{
    public partial class ChatRelation
    {
        public long ChatId { get; set; }
        public long ContactId { get; set; }
        public long? Pgrole { get; set; }

        public virtual ChatInfo Chat { get; set; }
        public virtual Contact Contact { get; set; }
    }
}
