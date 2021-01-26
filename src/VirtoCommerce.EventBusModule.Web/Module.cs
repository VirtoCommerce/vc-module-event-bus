using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using VirtoCommerce.Platform.Core.Modularity;

namespace VirtoCommerce.EventBusModule.Web
{
    public class Module : IModule
    {
        public ManifestModuleInfo ModuleInfo { get; set; }

        public void Initialize(IServiceCollection serviceCollection)
        {
            //Nothing
        }

        public void PostInitialize(IApplicationBuilder appBuilder)
        {
            //Nothing
        }

        public void Uninstall()
        {
            //Nothing
        }
    }
}
