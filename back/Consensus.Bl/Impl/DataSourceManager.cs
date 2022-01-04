using Consensus.Bl.Api;
using Consensus.Data.Entities;
using Consensus.Data;
using Consensus.DataSourceHandlers.Api;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;
using Consensus.Common.Configuration;
using Nest;
using Serilog;
using Consensus.Common.Exceptions;

namespace Consensus.Bl.Impl
{
    public class DataSourceManager : IDataSourceManager
    {
        private readonly SysConfig _sysConfig;
        private readonly IEnumerable<IDataSourceHandler> _handlers;
        private readonly ConsensusDbContext _dbContext;

        public DataSourceManager(IEnumerable<IDataSourceHandler> handlers, ConsensusDbContext dbContext, SysConfig sysConfig)
        {
            _handlers = handlers;
            _dbContext = dbContext;
            _sysConfig = sysConfig;
        }

        public async Task<Uri> InitCallback(string dataSourceCode, string propsJson)
        {
            var handler = GetHandler(dataSourceCode);
            var config = GetHandlerConfig(handler);
            var props = JsonConvert.DeserializeObject(propsJson, handler.TProps);

            var pipe = new Pipe
            {
                PublicId = Guid.NewGuid(),
                DataSourceCode = dataSourceCode,
                PropsJson = propsJson,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            };
            _dbContext.Pipes.Add(pipe);
            await _dbContext.SaveChangesAsync();

            var callbackUrl = new Uri($"{_sysConfig.BackEndUrl}/callback/{pipe.PublicId}");

            return await handler.InitCallback(config, props, callbackUrl);
        }

        public async Task HandleCallback(Guid pipeId, Uri callbackUrl)
        {
            var pipe = await _dbContext.Pipes.FirstOrDefaultAsync(p => p.PublicId == pipeId);
            if (pipe == null)
            {
                throw new ConsensusException($"Pipe not found: {pipeId}");
            }
            var handler = GetHandler(pipe.DataSourceCode);
            var config = GetHandlerConfig(handler);
            var props = JsonConvert.DeserializeObject(pipe.PropsJson);

            Log.Information("Opening pipe: {config}, {props}", JsonConvert.SerializeObject(config), pipe.PropsJson);

            var state = await handler.HandleCallback(config, props, callbackUrl);
            pipe.StateJson = JsonConvert.SerializeObject(state);
            pipe.Status = PipeStatus.Open;
            pipe.UpdatedAt = DateTime.UtcNow;
            if (pipe.FirstOpenedAt == null)
            {
                pipe.FirstOpenedAt = DateTime.UtcNow;
            }

            Log.Information("Opened pipe: {state}", pipe.StateJson);

            await _dbContext.SaveChangesAsync();
        }

        public async Task PumpDocuments(string dataSourceCode)
        {
            Log.Information("Pumping data from source: {dataSourceCode}", dataSourceCode);

            var handler = GetHandler(dataSourceCode);
            var config = GetHandlerConfig(handler);
            var dataSourceConfig = GetDataSourceConfig(dataSourceCode);

            var timeoutDate = DateTime.UtcNow.Add(-dataSourceConfig.Timeout);
            var pipe = await _dbContext.Pipes
                // Either open and not pumping or pumping for too long
                .Where(p => p.Status == PipeStatus.Open || (p.Status == PipeStatus.Pumping && p.LastPumpedAt < timeoutDate))
                .OrderBy(p => p.LastPumpedAt ?? DateTime.MinValue)
                .FirstOrDefaultAsync();
            if (pipe == null)
            {
                Log.Information("No applicable pipe found.");
                return;
            }

            if (pipe.Status == PipeStatus.Pumping)
            {
                Log.Error("Pipe {pipe} was stuck since {LastPumpedAt}. Clearing.", pipe.Id, pipe.LastPumpedAt);
                pipe.LastPumpedAt = DateTime.UtcNow;
            }

            Log.Information("Pumping data from source: {props}", pipe.PropsJson);

            var props = JsonConvert.DeserializeObject(pipe.PropsJson, handler.TProps);
            var state = JsonConvert.DeserializeObject(pipe.StateJson, handler.TState);

            pipe.Status = PipeStatus.Pumping;
            await _dbContext.SaveChangesAsync();
            try
            {
                var (documents, newState) = await handler.PumpDocuments(config, props, state);
                pipe.StateJson = JsonConvert.SerializeObject(newState);
                await SaveToElastic(documents);
                Log.Information("Successfully pumped {cnt} documents", documents.Length);
            }
            finally
            {
                pipe.Status = PipeStatus.Open;
                pipe.LastPumpedAt = DateTime.UtcNow;
                await _dbContext.SaveChangesAsync();
            }
        }

        private DataSourceConfig GetDataSourceConfig(string dataSourceCode)
        {
            return _sysConfig.ConsensusDataSources[dataSourceCode];
        }

        private object GetHandlerConfig(IDataSourceHandler handler)
        {
            return GetDataSourceConfig(handler.Code).Config.Get(handler.TConfig);
        }

        private IDataSourceHandler GetHandler(string dataSourceCode)
        {
            var result = _handlers.FirstOrDefault(h => h.Code == dataSourceCode);
            if (result == null)
            {
                throw new ConsensusException($"Data source {dataSourceCode} is not supported.");
            }

            return result;
        }

        private async Task SaveToElastic(ConsensusDocument[] documents)
        {
            var node = new Uri(_sysConfig.ElasticEndpoint);
            var settings = new ConnectionSettings(node);
            var client = new ElasticClient(settings);
            var groupsByIndex = documents.GroupBy(d => d.Source);
            foreach (var group in groupsByIndex)
            {
                var r = await client.BulkAsync(b => b
                    .Index($"consensus_{group.Key}_{DateTime.Now.Date:yyyy.MM}".ToLower())
                    .IndexMany(group));
            }
        }
    }
}
