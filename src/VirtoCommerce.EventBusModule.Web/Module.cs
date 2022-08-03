using System;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VirtoCommerce.EventBusModule.Core;
using VirtoCommerce.EventBusModule.Core.Services;
using VirtoCommerce.EventBusModule.Data.Repositories;
using VirtoCommerce.EventBusModule.Data.Services;
using VirtoCommerce.Platform.Core.Modularity;
using VirtoCommerce.Platform.Core.Security;
using VirtoCommerce.Platform.Core.GenericCrud;
using VirtoCommerce.Platform.Data.GenericCrud;
using VirtoCommerce.EventBusModule.Core.Models;
using VirtoCommerce.EventBusModule.Core.Options;

namespace VirtoCommerce.EventBusModule.Web
{
    public class Module : IModule, IHasConfiguration
    {
        public IConfiguration Configuration { get; set; }

        public ManifestModuleInfo ModuleInfo { get; set; }

        public void Initialize(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IEventBusRepository, EventBusRepository>();
            serviceCollection.AddDbContext<EventBusDbContext>((provider, options) =>
                options.UseSqlServer(provider.GetRequiredService<IConfiguration>().GetConnectionString("VirtoCommerce")));

            serviceCollection.AddTransient<Func<IEventBusRepository>>(provider => () => provider.CreateScope().ServiceProvider.GetRequiredService<IEventBusRepository>());
            
            serviceCollection.AddTransient<ISearchService<SubscriptionSearchCriteria, SubscriptionSearchResult, Subscription>, SubscriptionSearchService>();
            serviceCollection.AddTransient<ICrudService<Subscription>, SubscriptionService>();

            serviceCollection.AddTransient<ISearchService<ProviderConnectionSearchCriteria, ProviderConnectionSearchResult, ProviderConnection>, ProviderConnectionSearchService>();
            serviceCollection.AddTransient<ICrudService<ProviderConnection>, ProviderConnectionService>();

            serviceCollection.AddTransient<ICrudService<ProviderConnectionLog>, ProviderConnectionLogService>();

            serviceCollection.AddSingleton<IEventBusSubscriptionsManager, DefaultEventBusSubscriptionsManager>();

            serviceCollection.AddSingleton<IEventBusProviderConnectionsService, EventBusProviderConnectionsService>();

            serviceCollection.AddSingleton<IEventBusReadConfigurationService, EventBusReadConfigurationService>();
            
            serviceCollection.AddSingleton<IEventBusProviderService, EventBusProviderService>();

            serviceCollection.AddSingleton<RegisteredEventService>();

            serviceCollection.AddSingleton<IEventBusSubscriptionsService, EventBusSubscriptionsService>();

            var cfg = Configuration.GetSection("EventBus");
            serviceCollection.AddOptions<EventBusOptions>().Bind(cfg).ValidateDataAnnotations();
        }

        public void PostInitialize(IApplicationBuilder appBuilder)
        {
            //Register module permissions
            var permissionsProvider = appBuilder.ApplicationServices.GetRequiredService<IPermissionsRegistrar>();
            permissionsProvider.RegisterPermissions(ModuleConstants.Security.Permissions.AllPermissions.Select(x =>
                new Permission() { GroupName = "EventBus", Name = x }).ToArray());

            var webHookManager = appBuilder.ApplicationServices.GetService<IEventBusSubscriptionsManager>();
            webHookManager.RegisterEvents();

            //Force migrations
            using (var serviceScope = appBuilder.ApplicationServices.CreateScope())
            {
                var dbContext = serviceScope.ServiceProvider.GetRequiredService<EventBusDbContext>();
                dbContext.Database.EnsureCreated();
                dbContext.Database.Migrate();
            }

            //register Azure Event Grid provider
            var eventBusProviderService = appBuilder.ApplicationServices.GetRequiredService<IEventBusProviderService>();
            eventBusProviderService.RegisterProvider<AzureEventBusProvider>("AzureEventGrid");
        }

        public void Uninstall()
        {
            //Nothing
        }
    }
}
