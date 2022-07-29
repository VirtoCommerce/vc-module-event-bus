using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace VirtoCommerce.EventBusModule.Data.Repositories
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<EventBusDbContext>
    {
        public EventBusDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<EventBusDbContext>();

            builder.UseSqlServer("Data Source=(local);Initial Catalog=VirtoCommerce3;Persist Security Info=True;User ID=virto;Password=virto;MultipleActiveResultSets=True;Connect Timeout=30");

            return new EventBusDbContext(builder.Options);
        }
    }
}
