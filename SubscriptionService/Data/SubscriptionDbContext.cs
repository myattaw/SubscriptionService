namespace SubscriptionService.Data;

using Microsoft.EntityFrameworkCore;
using Models;

public class SubscriptionDbContext(DbContextOptions<SubscriptionDbContext> options) : DbContext(options)
{
    
    public DbSet<Subscription> Subscriptions { get; set; }
    
}