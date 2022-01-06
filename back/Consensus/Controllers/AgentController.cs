using AutoMapper;
using Consensus.ApiContracts;
using Consensus.Bl.Api;
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

        [HttpPost("agent/documents")]
        public async Task<IActionResult> PostDocuments([FromBody] AgentDocuments agentDocuments)
        {
            await _dataSourceManager.PumpDocumentsFromAgent(agentDocuments.PipeId, agentDocuments.StateJson, agentDocuments.Documents);

            return Ok("Ok");
        }

        [HttpPost("agent/logs")]
        public async Task<IActionResult> PostLogs([FromBody] AgentLog log)
        {
            Log.Write((Serilog.Events.LogEventLevel)log.Level, log.Message, log.Params.ToArray());

            return Ok("Ok");
        }
    }
}
