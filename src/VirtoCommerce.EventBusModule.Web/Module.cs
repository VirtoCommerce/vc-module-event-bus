using System;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VirtoCommerce.EventBusModule.Core;
using VirtoCommerce.EventBusModule.Core.Models;
using VirtoCommerce.EventBusModule.Core.Options;
using VirtoCommerce.EventBusModule.Core.Services;
using VirtoCommerce.EventBusModule.Data.MySql;
using VirtoCommerce.EventBusModule.Data.PostgreSql;
using VirtoCommerce.EventBusModule.Data.Repositories;
using VirtoCommerce.EventBusModule.Data.Services;
using VirtoCommerce.EventBusModule.Data.SqlServer;
using VirtoCommerce.Platform.Core.GenericCrud;
using VirtoCommerce.Platform.Core.Modularity;
using VirtoCommerce.Platform.Core.Security;

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
            {
                var databaseProvider = Configuration.GetValue("DatabaseProvider", "SqlServer");
                var connectionString = Configuration.GetConnectionString(ModuleInfo.Id) ?? Configuration.GetConnectionString("VirtoCommerce");

                switch (databaseProvider)
                {
                    case "MySql":
                        options.UseMySqlDatabase(connectionString);
                        break;
                    case "PostgreSql":
                        options.UsePostgreSqlDatabase(connectionString);
                        break;
                    default:
                        options.UseSqlServerDatabase(connectionString);
                        break;
                }
            });
            serviceCollection.AddTransient<Func<IEventBusRepository>>(provider => () => provider.CreateScope().ServiceProvider.GetRequiredService<IEventBusRepository>());

            serviceCollection.AddTransient<ISearchService<SubscriptionSearchCriteria, SubscriptionSearchResult, Subscription>, SubscriptionSearchService>();
            serviceCollection.AddTransient<ICrudService<Subscription>, SubscriptionService>();

            serviceCollection.AddTransient<ISearchService<ProviderConnectionSearchCriteria, ProviderConnectionSearchResult, ProviderConnection>, ProviderConnectionSearchService>();
            serviceCollection.AddTransient<ICrudService<ProviderConnection>, ProviderConnectionService>();

            serviceCollection.AddTransient<ISearchService<ProviderConnectionLogSearchCriteria, ProviderConnectionLogSearchResult, ProviderConnectionLog>, ProviderConnectionLogSearchService>();
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
