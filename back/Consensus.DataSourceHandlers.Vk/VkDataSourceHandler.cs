using Consensus.DataSourceHandlers.Api;
using Microsoft.AspNetCore.WebUtilities;
using System.Net.Http.Json;
using VkNet;
using VkNet.Model;
using VkNet.Model.RequestParams;

namespace Consensus.DataSourceHandlers.Vk
{
    public class VkDataSourceHandler : DataSourceHandlerBase<VkConfig, VkProps, VkState>
    {
        private static HttpClient _httpClient = new HttpClient();
        public override string Code => "Vk";

        public override async Task<Uri> InitCallback(VkConfig config, VkProps props, Uri callbackUrl)
        {
            var redirectUrl = $"https://oauth.vk.com/authorize?client_id={config.ClientId}&display=page&redirect_uri={callbackUrl}&scope={config.Scope}&response_type=code";
            return new Uri(redirectUrl);
        }

        public override async Task<VkState> HandleCallback(VkConfig config, VkProps props, Uri callbackUrl)
        {
            var redirectUrlParam = callbackUrl.GetLeftPart(UriPartial.Path);
            var query = QueryHelpers.ParseQuery(callbackUrl.Query);
            var code = query["code"][0];

            var url = $"https://oauth.vk.com/access_token?client_id={config.ClientId}&client_secret={config.ClientSecret}&redirect_uri={redirectUrlParam}&code={code}";
            var accessTokenResponse = await _httpClient.GetFromJsonAsync<AccessTokenResponse>(url);

            return new VkState
            {
                AccessToken = accessTokenResponse.access_token,
                RefreshToken = accessTokenResponse.refresh_token,
            };
        }

        public override async Task<(ConsensusDocument[], VkState)> PumpDocuments(VkConfig config, VkProps props, VkState state)
        {
            if (state.IsHistoryDone)
            {
                state.Offset = 0;
            }

            var api = new VkApi
            {
                // https://vk.com/dev/api_requests
                RequestsPerSecond = 3
            };
            await api.AuthorizeAsync(new ApiAuthParams
            {
                // TODO: there were some issues with using access_token - some weird API limits
                // with service token everything works fine and this is actually enough - public groups provide enough data
                // but in order to pump data from private groups - need to fix access_token issues somehow
                AccessToken = config.ServiceKey,
            });
            var messages = await api.Wall.GetAsync(new WallGetParams { Count = 100, Domain = props.CommunityName, Offset = state.Offset });
            if (messages.WallPosts.Count == 0)
            {
                state.IsHistoryDone = true;
                return (new ConsensusDocument[0], state);
            }

            var wallPosts = messages.WallPosts.ToList();

            if (state.IsHistoryDone)
            {
                wallPosts = wallPosts.Where(p => p.Date != null && p.Date > state.LastPostDate).ToList();
            }
            else
            {
                state.Offset += 100;
            }

            var postDocuments = wallPosts.Select(post =>
            {
                if (post.Date != null && post.Date > state.LastPostDate)
                {
                    state.LastPostDate = post.Date.Value;
                }
                var document = new ConsensusDocument
                {
                    Id = Guid.NewGuid(),
                    Source = Code,
                    Content = post.Text,
                    CreatedById = post.FromId?.ToString(),
                    CreatedAt = DateTime.UtcNow,
                    ExternalCreatedAt = post.Date,
                    ExternalSource = props.CommunityName,
                    ExternalId = post.Id.ToString(),
                    Url = $"https://vk.com/wall{post.OwnerId}_{post.Id}"
                };
                return (post, document);
            }).ToArray();

            var documents = new List<ConsensusDocument>();

            // No task/parallel stuff on purpose - in order not to meet API request limits
            foreach (var (post, postDocument) in postDocuments)
            {
                documents.Add(postDocument);

                var comments = new List<Comment>();
                for (var commentOffset = 0; commentOffset <= 1000; commentOffset += 100)
                {
                    var commentsPage = await api.Wall.GetCommentsAsync(new WallGetCommentsParams
                    {
                        OwnerId = post.OwnerId,
                        PostId = post.Id.Value,
                        Count = 100,
                        Offset = commentOffset,
                        Sort = VkNet.Enums.SortOrderBy.Desc,
                    });
                    if (commentsPage.Items.Count == 0)
                    {
                        break;
                    }
                    comments.AddRange(commentsPage.Items);
                }

                foreach (var comment in comments)
                {
                    var commentDocument = new ConsensusDocument
                    {
                        Id = Guid.NewGuid(),
                        Source = Code,
                        ExternalSource = props.CommunityName,
                        Content = comment.Text,
                        CreatedById = comment.FromId?.ToString(),
                        CreatedAt = DateTime.UtcNow,
                        ExternalCreatedAt = comment.Date,
                        ParentId = postDocument.Id,
                        ExternalId = comment.Id.ToString(),
                        Url = $"https://vk.com/wall{post.OwnerId}_{post.Id}?reply={comment.Id}",
                    };
                    documents.Add(commentDocument);
                }
            }

            // Need to pump only last X months of history
            if (props.PumpHistoryDays > 0)
            {
                var minDate = DateTime.UtcNow.AddDays(-props.PumpHistoryDays);
                if (documents.Any(d => d.ExternalCreatedAt <= minDate))
                {
                    state.IsHistoryDone = true;
                }
            }

            return (documents.ToArray(), state);
        }
    }
}
