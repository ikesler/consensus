using Autofac;
using Consensus.Common.Configuration;
using Microsoft.Extensions.Configuration;

namespace Consensus.Common
{
    public class CommonModule : Module
    {
        private readonly SysConfig _sysConfig;

        public CommonModule(IConfiguration appSettings)
        {
            _sysConfig = appSettings.Get<SysConfig>();
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterInstance(_sysConfig);
        }
    }
}