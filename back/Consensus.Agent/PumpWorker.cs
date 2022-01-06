using Consensus.ApiContracts;
using Consensus.DataSourceHandlers.Api;
using Microsoft.Extensions.Hosting;
using Serilog;
using System.Text.Json;

namespace Consensus.Agent
{
    public class PumpWorker : BackgroundService
    {
        private readonly IEnumerable<IDataSourceHandler> _dataSourceHandlers;
        private readonly IAgentApi _api;
        private readonly PureManClickOnce _deployment;

        public PumpWorker(IEnumerable<IDataSourceHandler> dataSourceHandlers, IAgentApi api, PureManClickOnce deployment)
        {
            _dataSourceHandlers = dataSourceHandlers;
            _api = api;
            _deployment = deployment;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (true)
            {
                try
                {
                   //  Log.Debug("Agent is awake");

                    try
                    {
                        await _deployment.Update();
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, "An error occurred while updating");
                    }

                    var supportedDataSources = _dataSourceHandlers.Select(x => x.Code).ToArray();
                    var pipes = await _api.GetPipes(supportedDataSources, stoppingToken);

                    foreach (var pipe in pipes)
                    {
                        try
                        {
                            throw new Exception("LOL :))))", new Exception("inner lol"));
                            Log.Debug("Agent is pumping data to {Source}/{Pipe} pipe", pipe.DataSourceCode, pipe.PublicId);

                            var handler = _dataSourceHandlers.Single(x => x.Code == pipe.DataSourceCode);
                            var (documents, newState) = await handler.PumpDocuments(null, pipe.PropsJson, pipe.StateJson);

                            Log.Debug($"Agent sending {documents.Length} documents to the server");

                            await _api.PostDocuments(new AgentDocuments
                            {
                                PipeId = pipe.PublicId,
                                StateJson = JsonSerializer.Serialize(newState),
                                Documents = documents,
                            }, stoppingToken);

                            Log.Debug($"Agent has successfully sent {documents.Length} documents to the server");
                        }
                        catch (Exception ex)
                        {
                            Log.Error(ex, "An error occurred in an agent when processing pipe {Pipe}", pipe.PublicId);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "An error occurred in an agent");
                }

                await Task.Delay(60 * 1000, stoppingToken);
            }
        }
    }
}
