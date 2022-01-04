using System;
using System.Collections.Generic;

namespace Consensus.Agents.Viber.DAL
{
    public partial class EventInfo
    {
        public long? EventId { get; set; }
        public long? TimeStamp { get; set; }
        public long? Direction { get; set; }
        public long? EventType { get; set; }
        public byte[] EventToken { get; set; }
        public byte[] SortOrder { get; set; }
        public long? Seq { get; set; }
        public byte[] IsRead { get; set; }
        public byte[] ContactLongitude { get; set; }
        public byte[] ContactLatitude { get; set; }
        public long? ChatId { get; set; }
        public byte[] MessageType { get; set; }
        public byte[] MessageStatus { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public byte[] Flag { get; set; }
        public byte[] ClientFlag { get; set; }
        public string PayloadPath { get; set; }
        public string ThumbnailPath { get; set; }
        public byte[] StickerId { get; set; }
        public byte[] PttId { get; set; }
        public byte[] PttStatus { get; set; }
        public byte[] Duration { get; set; }
        public byte[] CallType { get; set; }
        public byte[] CallStatus { get; set; }
        public long? ContactId { get; set; }
        public long? EventFlags { get; set; }
        public long? ChatFlags { get; set; }
        public byte[] PgmessageId { get; set; }
        public byte[] PgisLiked { get; set; }
        public byte[] PglikeCount { get; set; }
        public string MessageInfo { get; set; }
        public byte[] MessageAppId { get; set; }
        public byte[] FollowersLikeCount { get; set; }
        public string AdminsReactions { get; set; }
        public string MembersReactions { get; set; }
        public byte[] PrevReaction { get; set; }
    }
}
