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
        private readonly Deployment _deployment;
        private readonly IHostApplicationLifetime _applicationLifetime;

        public PumpWorker(IEnumerable<IDataSourceHandler> dataSourceHandlers, IAgentApi api, Deployment deployment, IHostApplicationLifetime applicationLifetime)
        {
            _dataSourceHandlers = dataSourceHandlers;
            _api = api;
            _deployment = deployment;
            _applicationLifetime = applicationLifetime;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (true)
            {
                try
                {
                    Log.Debug("Agent is awake");

                    try
                    {
                        if (await _deployment.CheckForUpdates())
                        {
                            await _deployment.Update();
                            _applicationLifetime.StopApplication();
                            return;
                        }
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
                            Log.Debug("Agent is pumping data to {Source}/{Pipe} pipe", pipe.DataSourceCode, pipe.PublicId);

                            var handler = _dataSourceHandlers.Single(x => x.Code == pipe.DataSourceCode);
                            var (documents, newState) = await handler.PumpDocuments(null, pipe.PropsJson, pipe.StateJson);

                            Log.Debug("Agent pumped {NumOfDocs} documents", documents.Length);
                            if (documents.Any())
                            {
                                await _api.PostDocuments(new AgentDocuments
                                {
                                    PipeId = pipe.PublicId,
                                    StateJson = JsonSerializer.Serialize(newState),
                                    Documents = documents,
                                }, stoppingToken);

                                Log.Debug("Documents have been sent to the API");
                            }
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

                // TODO: add Quartz and take schedules from the API
                await Task.Delay(60_000, stoppingToken);
            }
        }
    }
}
