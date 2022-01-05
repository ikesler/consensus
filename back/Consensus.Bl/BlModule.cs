using Autofac;
using Consensus.Bl.Impl;
using Consensus.DataSourceHandlers.Viber;
using Consensus.DataSourceHandlers.Vk;

namespace Consensus.Bl
{
    public class BlModule: Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<DataSourceManager>().AsImplementedInterfaces();
            builder.RegisterType<VkDataSourceHandler>().AsImplementedInterfaces();
            builder.RegisterType<ViberDataSourceHandler>().AsImplementedInterfaces();
        }
    }
}