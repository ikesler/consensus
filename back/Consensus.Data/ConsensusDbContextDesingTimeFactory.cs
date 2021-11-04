using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Consensus.Data
{
    public class ConsensusDbContextDesingTimeFactory : IDesignTimeDbContextFactory<ConsensusDbContext>
    {
        public ConsensusDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ConsensusDbContext>();
            optionsBuilder.UseNpgsql("Server=localhost;Database=consensus;User Id=postgres;Password=1;");

            return new ConsensusDbContext(optionsBuilder.Options);
        }
    }
}
