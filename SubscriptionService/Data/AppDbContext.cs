using SubscriptionService.Models.Payment;

namespace SubscriptionService.Data;

using Microsoft.EntityFrameworkCore;
using Models;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<SubscriptionPlan> SubscriptionPlans { get; set; }
    public DbSet<UserSubscription> UserSubscriptions { get; set; }

    public DbSet<PaymentTransaction> PaymentTransactions { get; set; }

    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Unique email per user
        modelBuilder.Entity<User>()
            .HasIndex(x => x.Email)
            .IsUnique();

        // UserSubscription -> User
        modelBuilder.Entity<UserSubscription>()
            .HasOne(x => x.User)
            .WithMany(x => x.UserSubscriptions)
            .HasForeignKey(x => x.UserId);

        // UserSubscription -> SubscriptionPlan
        modelBuilder.Entity<UserSubscription>()
            .HasOne(x => x.SubscriptionPlan)
            .WithMany(x => x.UserSubscriptions)
            .HasForeignKey(x => x.SubscriptionPlanId);
        
        modelBuilder.Entity<PaymentTransaction>()
            .HasOne(x => x.UserSubscription)
            .WithMany(x => x.PaymentTransactions)
            .HasForeignKey(x => x.UserSubscriptionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
    
}