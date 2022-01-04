using System;
using System.Collections.Generic;

namespace Consensus.Agents.Viber.DAL
{
    public partial class LikeRelation
    {
        public long MessageToken { get; set; }
        public long LikeEventId { get; set; }

        public virtual Event LikeEvent { get; set; }
    }
}
