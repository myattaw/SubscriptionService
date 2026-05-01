using Microsoft.EntityFrameworkCore;
using SubscriptionService.Data;
using SubscriptionService.Models.Payment;
using SubscriptionService.Models.Subscription;

namespace SubscriptionService.Services.Billing;

public class BillingCycleService : BackgroundService
{
    private readonly ILogger<BillingCycleService> _logger;
    private readonly IServiceScopeFactory _scopeFactory;

    public BillingCycleService(
        ILogger<BillingCycleService> logger,
        IServiceScopeFactory scopeFactory)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await RunBillingCycle();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Billing cycle failed.");
            }

            await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
        }
    }

    private async Task RunBillingCycle()
    {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var dueSubscriptions = await context.UserSubscriptions
            .Include(x => x.User)
            .Include(x => x.SubscriptionPlan)
            .Where(x =>
                x.BillingActive &&
                x.Status == SubscriptionStatus.Active &&
                x.NextBillingDate <= DateTime.UtcNow)
            .ToListAsync();

        foreach (var sub in dueSubscriptions)
        {
            if (sub.User.AccountCredits >= sub.CurrentPrice)
            {
                sub.User.AccountCredits -= sub.CurrentPrice;

                sub.NextBillingDate = DateTime.UtcNow.AddMonths(1);
                sub.Status = SubscriptionStatus.Active;

                context.PaymentTransactions.Add(new PaymentTransaction
                {
                    UserSubscriptionId = sub.Id,
                    Amount = sub.CurrentPrice,
                    ProcessedAt = DateTime.UtcNow,
                    Status = "Paid",
                    TransactionType = "RecurringCharge"
                });
            }
            else
            {
                sub.Status = SubscriptionStatus.PastDue;

                context.PaymentTransactions.Add(new PaymentTransaction
                {
                    UserSubscriptionId = sub.Id,
                    Amount = sub.CurrentPrice,
                    ProcessedAt = DateTime.UtcNow,
                    Status = "Failed",
                    TransactionType = "RecurringCharge",
                    FailureReason = "Insufficient account credits"
                });
            }
        }

        await context.SaveChangesAsync();
    }
}