using System;
using System.Collections.Generic;

namespace Consensus.Agents.Viber.DAL
{
    public partial class UploadFile
    {
        public long EventId { get; set; }
        public string ObjectId { get; set; }
        public string EncryptionParams { get; set; }
        public string Checksum { get; set; }

        public virtual Event Event { get; set; }
    }
}
