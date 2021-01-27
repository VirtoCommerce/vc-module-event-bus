using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VirtoCommerce.EventBusModule.Data.Model;
using VirtoCommerce.Platform.Data.Infrastructure;

namespace VirtoCommerce.EventBusModule.Data.Repositories
{
    public class SubscriptionRepository : DbContextRepositoryBase<SubscriptionDbContext>, ISubscriptionRepository
    {
        public SubscriptionRepository(SubscriptionDbContext dbContext) : base(dbContext)
        {
        }

        public IQueryable<SubscriptionEntity> Subscriptions => DbContext.Set<SubscriptionEntity>();

        public IQueryable<SubscriptionEventEntity> SubscriptionEvens => DbContext.Set<SubscriptionEventEntity>();
        
        public Task<SubscriptionEntity[]> GetSubscriptionsByIdsAsync(string[] ids) => Subscriptions
                .Where(x => ids.Contains(x.Id))
                .Include(x => x.Events)
                .ToArrayAsync();

        public async Task DeleteSubscriptionsByIdsAsync(string[] ids)
        {
            var subscriptions = await GetSubscriptionsByIdsAsync(ids);
            foreach (var subscription in subscriptions)
            {
                Remove(subscription);
            }
        }
    }
}
