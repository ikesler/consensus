using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
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

        public override ConsensusDocument[] RunJob(VkConfig config, VkProps props, VkState state)
        {
            var api = new VkApi();
            api.Authorize(new ApiAuthParams
            {
                AccessToken = state.AccessToken

            });
            var messages = api.Wall.Get(new WallGetParams { Count = 50, Domain = props.CommunityName, Extended = true });
            return messages.WallPosts.Select(p => new ConsensusDocument
            {
                Content = p.Text,
                CreatedById = p.FromId?.ToString(),
                ExternalCreatedAt = p.Date,
                SubSource = props.CommunityName,
            }).ToArray();
        }
    }
}
