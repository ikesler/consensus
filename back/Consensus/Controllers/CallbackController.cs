using Consensus.Bl.Api;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Consensus.Controllers
{
    public class CallbackController : ControllerBase
    {
        private readonly IDataSourceManager _dataSourceManager;

        public CallbackController(IDataSourceManager dataSourceManager)
        {
            _dataSourceManager = dataSourceManager;
        }

        [HttpGet("callback/{sourceCode}/redirect")]
        public async Task<IActionResult> Redirect([FromRoute] string sourceCode, [FromQuery(Name = "props")] string propsParam)
        {
            var redirectUrl = await _dataSourceManager.InitCallback(sourceCode, propsParam);

            if (redirectUrl == null)
            {
                return Ok("Ok");
            }

            return Redirect(redirectUrl.ToString());
        }

        [HttpGet("callback/{pipeId}")]
        public async Task<IActionResult> HandleCallback([FromRoute] Guid pipeId)
        {
            var url = new Uri(UriHelper.GetEncodedUrl(HttpContext.Request));
            await _dataSourceManager.HandleCallback(pipeId, url);

            return Ok("Ok");
        }
    }
}
