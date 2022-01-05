using Consensus.ApiContracts;
using Consensus.DataSourceHandlers.Api;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Consensus.Agent
{
    public class PumpWorker : BackgroundService
    {
        private readonly IEnumerable<IDataSourceHandler> _dataSourceHandlers;
        private readonly IAgentApi _api;
        private readonly ILogger<PumpWorker> _logger;

        public PumpWorker(IEnumerable<IDataSourceHandler> dataSourceHandlers, IAgentApi api, ILogger<PumpWorker> logger)
        {
            _dataSourceHandlers = dataSourceHandlers;
            _api = api;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (true)
            {
                try
                {
                    await _api.PostLogs(new AgentLog
                    {
                        Level = ApiContracts.LogLevel.Debug,
                        Message = "Agent is awake at machine {Machine}",
                        Params = new object[] { Environment.MachineName },
                    }, stoppingToken);

                    var supportedDataSources = _dataSourceHandlers.Select(x => x.Code).ToArray();
                    var pipes = await _api.GetPipes(supportedDataSources, stoppingToken);

                    foreach (var pipe in pipes)
                    {
                        try
                        {
                            await _api.PostLogs(new AgentLog
                            {
                                Level = ApiContracts.LogLevel.Debug,
                                Message = "Agent at machine {Machine} is pumping data to {Source}/{Pipe} pipe",
                                Params = new object[] { Environment.MachineName, pipe.DataSourceCode, pipe.PublicId },
                            }, stoppingToken);

                            var handler = _dataSourceHandlers.Single(x => x.Code == pipe.DataSourceCode);
                            var (documents, newState) = await handler.PumpDocuments(null, pipe.PropsJson, pipe.StateJson);

                            await _api.PostLogs(new AgentLog
                            {
                                Level = ApiContracts.LogLevel.Debug,
                                Message = $"Agent at machine {{Machine}} sending {documents.Length} documents to the server",
                                Params = new object[] { Environment.MachineName },
                            }, stoppingToken);

                            await _api.PostDocuments(new AgentDocuments
                            {
                                PipeId = pipe.PublicId,
                                StateJson = JsonSerializer.Serialize(newState),
                                Documents = documents,
                            }, stoppingToken);

                            await _api.PostLogs(new AgentLog
                            {
                                Level = ApiContracts.LogLevel.Debug,
                                Message = $"Agent at machine {{Machine}} has successfully sent {documents.Length} documents to the server",
                                Params = new object[] { Environment.MachineName },
                            }, stoppingToken);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, ex.Message);
                            await _api.PostLogs(new AgentLog
                            {
                                Level = ApiContracts.LogLevel.Error,
                                Message = $"An error occurred in an agent when processing pipe {{pipe}}: {ex}",
                                Params = new object [] { pipe.PublicId },
                            }, stoppingToken);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                    await _api.PostLogs(new AgentLog
                    {
                        Level = ApiContracts.LogLevel.Error,
                        Message = $"An error occurred in an agent: {ex}",
                    }, stoppingToken);
                }

                await Task.Delay(60 * 1000, stoppingToken);
            }
        }
    }
}
