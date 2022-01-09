using AutoMapper;
using Consensus.ApiContracts;
using Consensus.Bl.Api;
using Consensus.Models;
using Consensus.Utilities.Logging;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace Consensus.Controllers
{
    public class AgentController : ControllerBase
    {
        private readonly IDataSourceManager _dataSourceManager;
        private readonly IMapper _mapper;

        public AgentController(IDataSourceManager dataSourceManager, IMapper mapper)
        {
            _dataSourceManager = dataSourceManager;
            _mapper = mapper;
        }

        [HttpGet("agent/pipes")]
        public async Task<IActionResult> GetPipes([FromQuery(Name = "sources")] string[] sources)
        {
            var pipes = await _dataSourceManager.GetPipes(sources);
            return Ok(_mapper.Map<ApiContracts.Pipe[]>(pipes));
        }


        [HttpGet("agent/version")]
        public async Task<IActionResult> GetVersion()
        {
            var assemblyVersion = typeof(Startup).Assembly.GetName().Version;

            return Ok(new VersionModel { Version = assemblyVersion.ToString() });
        }

        [HttpGet("agent/download/{file}")]
        public async Task<IActionResult> DownloadExe(string file)
        {
            var exePath = Path.Combine(AppContext.BaseDirectory, "agent", Path.GetFileName(file));

            return PhysicalFile(exePath, "application/octet-stream", true);
        }

        [HttpPost("agent/documents")]
        public async Task<IActionResult> PostDocuments([FromBody] AgentDocuments agentDocuments)
        {
            await _dataSourceManager.PumpDocumentsFromAgent(agentDocuments.PipeId, agentDocuments.StateJson, agentDocuments.Documents);

            return Ok("Ok");
        }

        [HttpPost("agent/logs")]
        public async Task<IActionResult> PostLogs([FromBody] AgentLogEvents events)
        {
            foreach (var @event in events.Events)
            {
                var messageTemplate = string.IsNullOrWhiteSpace(@event.Exception)
                    ? @event.MessageTemplate
                    : @event.MessageTemplate + "\n" + @event.Exception;
                Log.Logger.ForContext(@event.Properties).Write(@event.Level, messageTemplate);
            }

            return Ok("Ok");
        }
    }
}
