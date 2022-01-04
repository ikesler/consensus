using Consensus.DataSourceHandlers.Api;
using Consensus.DataSourceHandlers.Viber.Db;
using Microsoft.EntityFrameworkCore;

namespace Consensus.DataSourceHandlers.Viber
{
    public class ViberDataSourceHandler : DataSourceHandlerBase<ViberConfig, ViberProps, ViberState>
    {
        public override string Code => "Viber";

        public override async Task<ViberState> HandleCallback(ViberConfig config, ViberProps props, Uri callbackUrl)
        {
            throw new NotImplementedException();
        }

        public override async Task<Uri> InitCallback(ViberConfig config, ViberProps props, Uri callbackUrl)
        {
            throw new NotImplementedException();
        }

        public override async Task<(ConsensusDocument[], ViberState)> PumpDocuments(ViberConfig config, ViberProps props, ViberState state)
        {
            var builder = new DbContextOptionsBuilder<ViberDbContext>();
            builder.UseSqlite(config.DbPath);

            var lastMessageTimeStamp = new DateTimeOffset(state.LastMessageDate).ToUnixTimeMilliseconds();

            using var db = new ViberDbContext(builder.Options);
            var messages = await db.Messages
                .Where(m => m.Event.TimeStamp > lastMessageTimeStamp)
                .Where(m => m.Event.Chat.Name == props.ChatName)
                .ToArrayAsync();

            var documents = messages.Select(m => new ConsensusDocument
            {
                Content = m.Body,
                ExternalId = m.EventId.ToString(),
                CreatedById = m.Event.ContactId.ToString(),
                CreatedByName = m.Event.Contact.Name,
                ExternalCreatedAt = DateTimeOffset.FromUnixTimeMilliseconds(m.Event.TimeStamp).DateTime,
                ExternalSource = props.ChatName,
            }).ToArray();

            state.LastMessageDate = DateTime.Now;
            return (documents, state);
        }
    }
}