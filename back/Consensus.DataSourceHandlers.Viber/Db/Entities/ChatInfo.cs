using System;
using System.Collections.Generic;

namespace Consensus.DataSourceHandlers.Viber.Db.Entities
{
    public partial class ChatInfo
    {
        public ChatInfo()
        {
            Events = new HashSet<Event>();
        }

        public long ChatId { get; set; }
        public string? Name { get; set; }
        public string? Token { get; set; }
        public long? Flags { get; set; }
        public long TimeStamp { get; set; }
        public string? IconId { get; set; }
        public string? BackgroundId { get; set; }
        public long? LastReadMessageToken { get; set; }
        public long? LastReadMessageId { get; set; }
        public long? LastSeenMessageToken { get; set; }
        public long? Pgtype { get; set; }
        public string? Pguri { get; set; }
        public long? Pgrevision { get; set; }
        public long? Pglongtitude { get; set; }
        public long? Pglatitude { get; set; }
        public string? Pgcountry { get; set; }
        public string? PgtabLine { get; set; }
        public string? Pgtags { get; set; }
        public long? PglastMessageId { get; set; }
        public long? PgwatchersCount { get; set; }
        public long? PgsearchFlags { get; set; }
        public string? MetaData { get; set; }
        public long? PgsearchExFlags { get; set; }

        public virtual ICollection<Event> Events { get; set; }
    }
}
