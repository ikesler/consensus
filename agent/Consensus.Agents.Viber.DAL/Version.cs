using System;
using System.Collections.Generic;

namespace Consensus.Agents.Viber.DAL
{
    public partial class Version
    {
        public long VersionId { get; set; }
        public string VersionNumber { get; set; }
        public long TimeStamp { get; set; }
        public long Status { get; set; }
        public string Title { get; set; }
    }
}
