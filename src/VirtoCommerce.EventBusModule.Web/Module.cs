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

namespace VirtoCommerce.EventBusModule.Web
{
    public class Module : IModule
    {
        public ManifestModuleInfo ModuleInfo { get; set; }

        public void Initialize(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<ISubscriptionRepository, SubscriptionRepository>();
            serviceCollection.AddDbContext<SubscriptionDbContext>((provider, options) =>
                options.UseSqlServer(provider.GetRequiredService<IConfiguration>().GetConnectionString("VirtoCommerce")));
            serviceCollection.AddTransient<Func<ISubscriptionRepository>>(provider => () => provider.CreateScope().ServiceProvider.GetRequiredService<ISubscriptionRepository>());

            serviceCollection.AddTransient<IEventBusSubscriptionsManager, DefaultEventBusSubscriptionsManager>();
            serviceCollection.AddTransient<ISubscriptionSearchService, SubscriptionSearchService>();
            serviceCollection.AddTransient<ISubscriptionService, SubscriptionService>();
            serviceCollection.AddSingleton<RegisteredEventService>();
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
                var dbContext = serviceScope.ServiceProvider.GetRequiredService<SubscriptionDbContext>();
                dbContext.Database.EnsureCreated();
                dbContext.Database.Migrate();
            }
        }

        public void Uninstall()
        {
            //Nothing
        }
    }
}
