using System;
using System.Collections.Generic;

namespace Consensus.Agents.Viber.DAL
{
    public partial class Reminder
    {
        public long ChatId { get; set; }
        public long EventId { get; set; }
        public long RepeatMode { get; set; }
        public long TimeStamp { get; set; }
        public long OriginalTimeStamp { get; set; }
        public long DismissTimeStamp { get; set; }

        public virtual ChatInfo Chat { get; set; }
        public virtual Event Event { get; set; }
    }
}
