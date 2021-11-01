using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using System.Diagnostics;
using VkNet;
using VkNet.Model;
using VkNet.Model.RequestParams;

namespace Consensus.DataSourceHandlers.Vk
{
    public class VkDataSourceHandler: DataSourceHandlerBase<VkConfig, VkProps, VkState>
    {
        public VkDataSourceHandler(SysConfig sysConfig) : base(sysConfig)
        {
        }

        public override string Code => "VkWall";

        public override (VkProps, VkState) HandleCallback(VkConfig config, Uri redirectUrl)
        {
            var redirectUrlParam = Uri.EscapeDataString(SysConfig.BackEndUrl + redirectUrl.AbsolutePath);
            var code = QueryHelpers.ParseQuery(redirectUrl.Query)["code"][0];
            var state = QueryHelpers.ParseQuery(redirectUrl.Query)["state"][0];

            var client = new HttpClient();
            var url = $"https://oauth.vk.com/access_token?client_id={config.ClientId}&client_secret={config.ClientSecret}&redirect_uri={redirectUrlParam}&code={code}";
            var accessTokenResponse = client.GetFromJsonAsync<AccessTokenResponse>(url).Result;

            var jobProps = JsonConvert.DeserializeObject<VkProps>(state);
            jobProps.UserId = accessTokenResponse.user_id;
            var jobState = new VkState
            {
                AccessToken = accessTokenResponse.access_token,
                RefreshToken = accessTokenResponse.refresh_token,
            };

            return (jobProps, jobState);
        }

        public override Uri InitCallback(VkConfig config, VkProps props)
        {
            var callbackUrl = Uri.EscapeDataString($"{SysConfig.BackEndUrl}/callback/{Code}");
            var state = Uri.EscapeDataString(JsonConvert.SerializeObject(props));
            var redirectUrl = $"https://oauth.vk.com/authorize?client_id={config.ClientId}&display=page&redirect_uri={callbackUrl}&scope={config.Scope}&response_type=code&state={state}";
            return new Uri(redirectUrl);
        }

        public override (ConsensusDocument[], VkState) RunJob(VkConfig config, VkProps props, VkState state)
        {
            var api = new VkApi();
            api.Authorize(new ApiAuthParams
            {
                AccessToken = config.ServiceKey,
            });
            var messages = api.Wall.Get(new WallGetParams { Count = 100, Domain = props.CommunityName, Offset = state.Offset });
            if (messages.WallPosts.Count == 0)
            {
                state.Offset = 0;
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

            foreach (var (post, postDocument) in postDocuments)
            {
                documents.Add(postDocument);

                var comments = new List<Comment>();
                for (var commentOffset = 0; commentOffset <= 1000; commentOffset += 100)
                {
                    var stopwatch = new Stopwatch();
                    stopwatch.Start();
                    var commentsPage = api.Wall.GetComments(new WallGetCommentsParams
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
                    stopwatch.Stop();
                    if (stopwatch.ElapsedMilliseconds < 200)
                    {
                        Thread.Sleep(200 - (int)stopwatch.ElapsedMilliseconds);
                    }
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

            return (documents.ToArray(), state);
        }
    }
}
