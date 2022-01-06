using Consensus.DataSourceHandlers.Api;
using Consensus.DataSourceHandlers.Viber.Db;
using Microsoft.EntityFrameworkCore;

namespace Consensus.DataSourceHandlers.Viber
{
    public class ViberDataSourceHandler : DataSourceHandlerBase<ViberConfig, ViberProps, ViberState>
    {
        private const string DbFileName = "viber.db";

        public override string Code => "Viber";

        public override async Task<ViberState> HandleCallback(ViberConfig config, ViberProps props, Uri callbackUrl)
        {
            return new ViberState();
        }

        public override async Task<Uri> InitCallback(ViberConfig config, ViberProps props, Uri callbackUrl)
        {
            return null;
        }

        public override async Task<(ConsensusDocument[], ViberState)> PumpDocuments(ViberConfig config, ViberProps props, ViberState state)
        {
            var homeDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            var viberDir = Path.Combine(homeDir, "AppData", "Roaming", "ViberPC");
            string? dbDir;
            if (!string.IsNullOrWhiteSpace(props.PhoneNumber))
            {
                dbDir = Path.Combine(homeDir, props.PhoneNumber);
            }
            else
            {
                dbDir = Directory.GetDirectories(viberDir).FirstOrDefault(d => File.Exists(Path.Combine(d, DbFileName)));
                if (dbDir == null)
                {
                    throw new InvalidOperationException("Phone number was not specified and Viber DB file was not found in default location");
                }
            }
            var dbFilePath = Path.Combine(dbDir, DbFileName);

            var builder = new DbContextOptionsBuilder<ViberDbContext>();
            builder.UseSqlite($"Data Source={dbFilePath};Mode=ReadOnly");

            var firstMessageTimeStamp = (state.FirstMessageDate ?? DateTimeOffset.MaxValue).ToUnixTimeMilliseconds();
            var lastMessageTimeStamp = (state.LastMessageDate ?? DateTimeOffset.MinValue).ToUnixTimeMilliseconds();

            using var db = new ViberDbContext(builder.Options);

            var messages = await db.Messages
                .Include(m => m.Event.Contact)
                .Where(m => m.Event.TimeStamp > lastMessageTimeStamp || m.Event.TimeStamp < firstMessageTimeStamp)
                .Where(m => m.Event.Chat.Name == props.ChatName)
                .ToArrayAsync();

            var documents = messages.Select(m => new ConsensusDocument
            {
                Id = Guid.NewGuid(),
                Source = Code,
                Content = m.Body,
                ExternalId = m.EventId.ToString(),
                CreatedById = m.Event.ContactId?.ToString(),
                CreatedByName = m.Event.Contact?.Name ?? m.Event.Contact?.ClientName,
                ExternalCreatedAt = DateTimeOffset.FromUnixTimeMilliseconds(m.Event.TimeStamp).DateTime,
                ExternalSource = props.ChatName,
            }).ToArray();

            if (messages.Any())
            {
                // Viber local DB does not hold complete chat history - only recent messages
                // Viber may add/remove that messages depending on the user behavior - the scroll state
                // This is the reason of these timestamp manipulations - we can expect new messages from the both sides of the timeline
                if (state.FirstMessageDate == null)
                {
                    state.FirstMessageDate = DateTimeOffset.FromUnixTimeMilliseconds(messages.Select(x => x.Event.TimeStamp).Min());
                }
                else
                {
                    state.FirstMessageDate = DateTimeOffset.FromUnixTimeMilliseconds(
                        messages.Select(x => x.Event.TimeStamp).Union(new[] { firstMessageTimeStamp }).Min()
                    );
                }

                if (state.LastMessageDate == null)
                {
                    state.LastMessageDate = DateTimeOffset.FromUnixTimeMilliseconds(messages.Select(x => x.Event.TimeStamp).Max());
                }
                else
                {
                    state.LastMessageDate = DateTimeOffset.FromUnixTimeMilliseconds(
                        messages.Select(x => x.Event.TimeStamp).Union(new[] { lastMessageTimeStamp }).Max()
                    );
                }
            }

            return (documents, state);
        }
    }
}