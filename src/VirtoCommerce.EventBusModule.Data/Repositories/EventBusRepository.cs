using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VirtoCommerce.EventBusModule.Data.Model;
using VirtoCommerce.Platform.Data.Infrastructure;

namespace VirtoCommerce.EventBusModule.Data.Repositories
{
    public class EventBusRepository : DbContextRepositoryBase<EventBusDbContext>, IEventBusRepository
    {
        public EventBusRepository(EventBusDbContext dbContext) : base(dbContext)
        {
        }

        public IQueryable<SubscriptionEntity> Subscriptions => DbContext.Set<SubscriptionEntity>();

        public IQueryable<SubscriptionEventEntity> SubscriptionEvents => DbContext.Set<SubscriptionEventEntity>();
        public IQueryable<ProviderConnectionEntity> ProviderConnections => DbContext.Set<ProviderConnectionEntity>();
        public IQueryable<ProviderConnectionLogEntity> ProviderConnectionLogs => DbContext.Set<ProviderConnectionLogEntity>();

        public Task<SubscriptionEntity[]> GetSubscriptionsByIdsAsync(IEnumerable<string> ids) => Subscriptions
                .Where(x => ids.Contains(x.Id))
                .Include(x => x.Events)
                .ToArrayAsync();

        public async Task DeleteSubscriptionsByIdsAsync(IEnumerable<string> ids)
        {
            var subscriptions = await GetSubscriptionsByIdsAsync(ids);
            foreach (var subscription in subscriptions)
            {
                Remove(subscription);
            }
        }

        public Task<ProviderConnectionEntity[]> GetProviderConnectionsByIdsAsync(IEnumerable<string> ids) => ProviderConnections
                .Where(x => ids.Contains(x.Id))
                .ToArrayAsync();

        public Task<ProviderConnectionLogEntity[]> GetProviderConnectionLogsByIdsAsync(IEnumerable<string> ids) => ProviderConnectionLogs
                .Where(x => ids.Contains(x.Id))
                .ToArrayAsync();
    }
}
