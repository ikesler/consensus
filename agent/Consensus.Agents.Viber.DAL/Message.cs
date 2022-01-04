using System;
using System.Collections.Generic;

namespace Consensus.Agents.Viber.DAL
{
    public partial class Message
    {
        public long EventId { get; set; }
        public long Type { get; set; }
        public long Status { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public long? Flag { get; set; }
        public string PayloadPath { get; set; }
        public string ThumbnailPath { get; set; }
        public byte[] StickerId { get; set; }
        public string PttId { get; set; }
        public byte[] PttStatus { get; set; }
        public byte[] Duration { get; set; }
        public byte[] PgmessageId { get; set; }
        public long? PgisLiked { get; set; }
        public long? PglikeCount { get; set; }
        public string Info { get; set; }
        public long? AppId { get; set; }
        public long? ClientFlag { get; set; }
        public long? FollowersLikeCount { get; set; }
        public string AdminsReactions { get; set; }
        public string MembersReactions { get; set; }
        public long? PrevReaction { get; set; }

        public virtual Event Event { get; set; }
    }
}
