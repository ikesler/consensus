using System;
using System.Collections.Generic;

namespace Consensus.DataSourceHandlers.Viber.Db.Entities
{
    public partial class Event
    {
        public long EventId { get; set; }
        public long TimeStamp { get; set; }
        public long Direction { get; set; }
        public long Type { get; set; }
        public byte[] ContactLongitude { get; set; }
        public byte[] ContactLatitude { get; set; }
        public long? ChatId { get; set; }
        public long? ContactId { get; set; }
        public long? IsSessionLifeTime { get; set; }
        public long? Flags { get; set; }
        public byte[] Token { get; set; }
        public long IsRead { get; set; }
        public byte[] SortOrder { get; set; }
        public long Seq { get; set; }

        public virtual ChatInfo Chat { get; set; }
        public virtual Contact Contact { get; set; }
        public virtual Message Message { get; set; }
    }
}
