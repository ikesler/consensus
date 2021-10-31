using Consensus.Commands.Tenant;
using Hangfire;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using VkNet;
using VkNet.Model;
using VkNet.Model.RequestParams;    

namespace Consensus.Controllers
{
    public class AccessTokenResponse
    {
        public string access_token { get; set; }
    }
    /*
    public class TenantController : ControllerBase
    {
        [HttpGet("tenants/callback/1")]
        public IActionResult Create([FromQuery] string code, [FromQuery] string state)
{
            var api = new VkApi();

            
            var clientId = ;
            var cliebtSecret = "";
            var scope = 1 << 18;
            var redirectUrl = "http://localhost:5254/tenants/callback/1"; // Uri.EscapeDataString($"http://localhost:5254/tenants/callback/1&scope={scope}&response_type=code&state={state}");

            var client = new HttpClient();
            var url = $"https://oauth.vk.com/access_token?client_id={clientId}&client_secret={cliebtSecret}&redirect_uri={redirectUrl}&code={code}";
            var accessTokenResponse = client.GetFromJsonAsync<AccessTokenResponse>(url).Result;

            api.Authorize(new ApiAuthParams
            {
                AccessToken = accessTokenResponse.access_token

            });
            var messages = api.Wall.Get(new WallGetParams { Count = 50, Domain = "ohsev", Extended = true });
            var cmd = JsonConvert.DeserializeObject<CreateTenant>(state);
            RecurringJob.AddOrUpdate(cmd.Name, () => Console.WriteLine(JsonConvert.SerializeObject(cmd)), Cron.Minutely());
            return Ok();
        }
    }*/
}
