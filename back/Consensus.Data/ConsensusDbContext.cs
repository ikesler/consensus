using Consensus.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Consensus.Data
{
    public class ConsensusDbContext : DbContext
    {
        public ConsensusDbContext(DbContextOptions<ConsensusDbContext> options) : base(options)
        {
        }

        public DbSet<Pipe> Pipes { get; set; }
    }
}
