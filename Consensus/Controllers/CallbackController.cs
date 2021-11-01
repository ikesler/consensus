using Microsoft.AspNetCore.Mvc;
using Consensus.DataSourceHandlers;
using Microsoft.AspNetCore.Http.Extensions;
using Newtonsoft.Json;
using Hangfire;

namespace Consensus.Controllers
{
    public class CallbackController: ControllerBase
    {
        private readonly IEnumerable<IDataSourceHandler> _handlers;
        private readonly IConfiguration _configuration;
        private readonly SysConfig _sysConfig;

        public CallbackController(IEnumerable<IDataSourceHandler> handlers, IConfiguration configuration, SysConfig sysConfig)
        {
            _handlers = handlers;
            _configuration = configuration;
            _sysConfig = sysConfig;
        }

        [HttpGet("callback/{sourceCode}/redirect")]
        public IActionResult Redirect([FromRoute] string sourceCode, [FromQuery(Name = "props")] string propsParam)
        {
            var handler = _handlers.Single(x => x.Code == sourceCode);
            var config = _configuration.GetSection($"DataSources:{sourceCode}")
                .Get(handler.TConfig);
            var props = JsonConvert.DeserializeObject(propsParam, handler.TProps);

            var redirectUrl = handler.InitCallback(config, props);
            return Redirect(redirectUrl.ToString());
        }

        [HttpGet("callback/{sourceCode}")]
        public IActionResult HandleCallback([FromRoute] string sourceCode)
        {
            var handler = _handlers.Single(x => x.Code == sourceCode);
            var config = _configuration.GetSection($"DataSources:{sourceCode}")
                .Get(handler.TConfig);
            var (props, state) = handler.HandleCallback(config, new Uri(UriHelper.GetEncodedUrl(HttpContext.Request)));
            RecurringJob.AddOrUpdate($"{sourceCode} {JsonConvert.SerializeObject(props)}",
                () => handler.RunJob(config, props, state), Cron.Minutely());

            return Redirect($"{_sysConfig.FrontEndUrl}/hangfire");
        }
    }
}
