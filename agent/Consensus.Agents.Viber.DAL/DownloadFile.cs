using System;
using System.Collections.Generic;

namespace Consensus.Agents.Viber.DAL
{
    public partial class DownloadFile
    {
        public string DownloadId { get; set; }
        public long EventId { get; set; }
        public long Type { get; set; }
        public long DownloadStatus { get; set; }
        public string TempFileName { get; set; }
        public string Checksum { get; set; }

        public virtual Event Event { get; set; }
    }
}
