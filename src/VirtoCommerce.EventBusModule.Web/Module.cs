using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VirtoCommerce.EventBusModule.Core;
using VirtoCommerce.EventBusModule.Core.Options;
using VirtoCommerce.EventBusModule.Core.Services;
using VirtoCommerce.EventBusModule.Data.MySql;
using VirtoCommerce.EventBusModule.Data.PostgreSql;
using VirtoCommerce.EventBusModule.Data.Repositories;
using VirtoCommerce.EventBusModule.Data.Services;
using VirtoCommerce.EventBusModule.Data.SqlServer;
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
            serviceCollection.AddDbContext<EventBusDbContext>(options =>
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

            serviceCollection.AddTransient<IEventBusRepository, EventBusRepository>();
            serviceCollection.AddTransient<Func<IEventBusRepository>>(provider => () => provider.CreateScope().ServiceProvider.GetRequiredService<IEventBusRepository>());

            serviceCollection.AddTransient<ISubscriptionSearchService, SubscriptionSearchService>();
            serviceCollection.AddTransient<ISubscriptionService, SubscriptionService>();

            serviceCollection.AddTransient<IProviderConnectionSearchService, ProviderConnectionSearchService>();
            serviceCollection.AddTransient<IProviderConnectionService, ProviderConnectionService>();

            serviceCollection.AddTransient<IProviderConnectionLogSearchService, ProviderConnectionLogSearchService>();
            serviceCollection.AddTransient<IProviderConnectionLogService, ProviderConnectionLogService>();

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
            // Register permissions
            var permissionsRegistrar = appBuilder.ApplicationServices.GetRequiredService<IPermissionsRegistrar>();
            permissionsRegistrar.RegisterPermissions(ModuleInfo.Id, "EventBus", ModuleConstants.Security.Permissions.AllPermissions);

            var eventBusSubscriptionsManager = appBuilder.ApplicationServices.GetService<IEventBusSubscriptionsManager>();
            eventBusSubscriptionsManager.RegisterEvents();

            // Register Azure Event Grid provider
            var eventBusProviderService = appBuilder.ApplicationServices.GetRequiredService<IEventBusProviderService>();
            eventBusProviderService.RegisterProvider<AzureEventBusProvider>("AzureEventGrid");

            // Apply migrations
            using var serviceScope = appBuilder.ApplicationServices.CreateScope();
            var dbContext = serviceScope.ServiceProvider.GetRequiredService<EventBusDbContext>();
            dbContext.Database.Migrate();
        }

        public void Uninstall()
        {
            //Nothing
        }
    }
}
