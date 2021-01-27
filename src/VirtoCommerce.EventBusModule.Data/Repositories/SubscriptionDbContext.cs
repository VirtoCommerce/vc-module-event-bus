using System;
using EntityFrameworkCore.Triggers;
using Microsoft.EntityFrameworkCore;
using VirtoCommerce.EventBusModule.Data.Model;

namespace VirtoCommerce.EventBusModule.Data.Repositories
{
    public class SubscriptionDbContext : DbContextWithTriggers
    {
        public SubscriptionDbContext(DbContextOptions<SubscriptionDbContext> options)
            : base(options)
        {
        }

        protected SubscriptionDbContext(DbContextOptions options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SubscriptionEntity>().HasKey(x => x.Id);
            modelBuilder.Entity<SubscriptionEntity>().ToTable("EventBusSubscription");
            modelBuilder.Entity<SubscriptionEntity>().Property(x => x.Id).HasMaxLength(128).ValueGeneratedOnAdd();

            modelBuilder.Entity<SubscriptionEventEntity>().HasKey(x => x.Id);
            modelBuilder.Entity<SubscriptionEventEntity>().ToTable("EventBusSubscriptionEvent");
            modelBuilder.Entity<SubscriptionEventEntity>().Property(x => x.Id).HasMaxLength(128).ValueGeneratedOnAdd();
            modelBuilder.Entity<SubscriptionEventEntity>().HasOne(m => m.Subscription).WithMany(m => m.Events)
                .HasForeignKey(m => m.SubscriptionId).OnDelete(DeleteBehavior.Cascade).IsRequired();
            modelBuilder.Entity<SubscriptionEventEntity>().HasIndex(i => i.SubscriptionId);

            base.OnModelCreating(modelBuilder);
        }
    }
}
