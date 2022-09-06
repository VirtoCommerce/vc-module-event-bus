using EntityFrameworkCore.Triggers;
using Microsoft.EntityFrameworkCore;
using VirtoCommerce.EventBusModule.Data.Model;

namespace VirtoCommerce.EventBusModule.Data.Repositories
{
    public class EventBusDbContext : DbContextWithTriggers
    {
        protected const int _idLength = 128;
        public EventBusDbContext(DbContextOptions<EventBusDbContext> options)
            : base(options)
        {
        }

        protected EventBusDbContext(DbContextOptions options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SubscriptionEntity>().HasKey(x => x.Id);
            modelBuilder.Entity<SubscriptionEntity>().ToTable("EventBus2Subscription");
            modelBuilder.Entity<SubscriptionEntity>().Property(x => x.Id).HasMaxLength(_idLength).ValueGeneratedOnAdd();

            modelBuilder.Entity<ProviderConnectionEntity>().HasKey(x => x.Id);
            modelBuilder.Entity<ProviderConnectionEntity>().ToTable("EventBus2ProviderConnection");
            modelBuilder.Entity<ProviderConnectionEntity>().Property(x => x.Id).HasMaxLength(_idLength).ValueGeneratedOnAdd();

            modelBuilder.Entity<ProviderConnectionLogEntity>().HasKey(x => x.Id);
            modelBuilder.Entity<ProviderConnectionLogEntity>().ToTable("EventBus2ProviderConnectionLog");
            modelBuilder.Entity<ProviderConnectionLogEntity>().Property(x => x.Id).HasMaxLength(_idLength).ValueGeneratedOnAdd();

            modelBuilder.Entity<SubscriptionEventEntity>().HasKey(x => x.Id);
            modelBuilder.Entity<SubscriptionEventEntity>().ToTable("EventBus2SubscriptionEvent");
            modelBuilder.Entity<SubscriptionEventEntity>().Property(x => x.Id).HasMaxLength(_idLength).ValueGeneratedOnAdd();
            modelBuilder.Entity<SubscriptionEventEntity>().HasOne(m => m.Subscription).WithMany(m => m.Events)
                .HasForeignKey(m => m.SubscriptionId).OnDelete(DeleteBehavior.Cascade).IsRequired();
            modelBuilder.Entity<SubscriptionEventEntity>().HasIndex(i => i.SubscriptionId);

            base.OnModelCreating(modelBuilder);
        }
    }
}
