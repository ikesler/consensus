using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace Consensus.Data
{
    public class ConsensusDbMigrator : IHostedService
    {
        private readonly ConsensusDbContext _dbContext;

        public ConsensusDbMigrator(ConsensusDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _dbContext.Database.MigrateAsync();
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            
        }
    }
}
