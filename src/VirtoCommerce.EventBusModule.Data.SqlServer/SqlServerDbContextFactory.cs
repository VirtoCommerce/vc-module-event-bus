using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using VirtoCommerce.EventBusModule.Data.Repositories;

namespace VirtoCommerce.EventBusModule.Data.SqlServer
{
    public class SqlServerDbContextFactory : IDesignTimeDbContextFactory<EventBusDbContext>
    {
        public EventBusDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<EventBusDbContext>();
            var connectionString = args.Any() ? args[0] : "Data Source=(local);Initial Catalog=VirtoCommerce3;Persist Security Info=True;User ID=virto;Password=virto;MultipleActiveResultSets=True;Connect Timeout=30";

            builder.UseSqlServer(
                connectionString,
                db => db.MigrationsAssembly(typeof(SqlServerDbContextFactory).Assembly.GetName().Name));

            return new EventBusDbContext(builder.Options);
        }
    }
}
