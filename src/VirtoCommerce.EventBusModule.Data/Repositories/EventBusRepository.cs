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
        public EventBusRepository(EventBusDbContext dbContext)
            : base(dbContext)
        {
        }

        public IQueryable<SubscriptionEntity> Subscriptions => DbContext.Set<SubscriptionEntity>();

        public IQueryable<SubscriptionEventEntity> SubscriptionEvents => DbContext.Set<SubscriptionEventEntity>();

        public IQueryable<ProviderConnectionEntity> ProviderConnections => DbContext.Set<ProviderConnectionEntity>();

        public IQueryable<ProviderConnectionLogEntity> ProviderConnectionLogs => DbContext.Set<ProviderConnectionLogEntity>();

        public async Task<IList<SubscriptionEntity>> GetSubscriptionsByIdsAsync(IList<string> ids)
        {
            return await Subscriptions
                .Where(x => ids.Contains(x.Id))
                .Include(x => x.Events)
                .ToListAsync();
        }

        public async Task DeleteSubscriptionsByIdsAsync(IList<string> ids)
        {
            var subscriptions = await GetSubscriptionsByIdsAsync(ids);
            foreach (var subscription in subscriptions)
            {
                Remove(subscription);
            }
        }

        public async Task<IList<ProviderConnectionEntity>> GetProviderConnectionsByIdsAsync(IList<string> ids)
        {
            return await ProviderConnections
                .Where(x => ids.Contains(x.Id))
                .ToListAsync();
        }

        public async Task<IList<ProviderConnectionLogEntity>> GetProviderConnectionLogsByIdsAsync(IList<string> ids)
        {
            return await ProviderConnectionLogs
                .Where(x => ids.Contains(x.Id))
                .ToListAsync();
        }
    }
}
