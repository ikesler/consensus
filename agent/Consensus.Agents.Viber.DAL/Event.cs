using System;
using System.Collections.Generic;

namespace Consensus.Agents.Viber.DAL
{
    public partial class Event
    {
        public Event()
        {
            DownloadFiles = new HashSet<DownloadFile>();
            EventsMetaData = new HashSet<EventsMetaDatum>();
            LikeRelations = new HashSet<LikeRelation>();
        }

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
        public virtual Call Call { get; set; }
        public virtual Message Message { get; set; }
        public virtual Reminder Reminder { get; set; }
        public virtual UploadFile UploadFile { get; set; }
        public virtual ICollection<DownloadFile> DownloadFiles { get; set; }
        public virtual ICollection<EventsMetaDatum> EventsMetaData { get; set; }
        public virtual ICollection<LikeRelation> LikeRelations { get; set; }
    }
}
