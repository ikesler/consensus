using Hangfire.Annotations;
using Hangfire.Dashboard;

namespace Consensus.Hangfire
{
    public class DashboardNoAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize([NotNull] DashboardContext context)
        {
            return true;
        }
    }
}
