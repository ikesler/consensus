using System;
using System.Collections.Generic;

namespace Consensus.DataSourceHandlers.Viber.Db.Entities
{
    public partial class Contact
    {
        public Contact()
        {
            Events = new HashSet<Event>();
        }

        public long ContactId { get; set; }
        public string Name { get; set; }
        public long Abcontact { get; set; }
        public long ViberContact { get; set; }
        public string Number { get; set; }
        public string Mid { get; set; }
        public string EncryptedNumber { get; set; }
        public string EncryptedMid { get; set; }
        public string Vid { get; set; }
        public string ClientName { get; set; }
        public string DownloadId { get; set; }
        public byte[] ContactFlags { get; set; }
        public string SortName { get; set; }
        public long? Timestamp { get; set; }
        public string DateOfBirth { get; set; }

        public virtual ICollection<Event> Events { get; set; }
    }
}
