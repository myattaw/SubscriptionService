using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using SubscriptionService.Data;
using SubscriptionService.Models.Payment;
using SubscriptionService.Models.Subscription;

namespace SubscriptionService.Services.Billing;

public class BillingService
{
    
    private readonly AppDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;
    
    public BillingService(AppDbContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<object?> GetPaymentHistoryAsync(int userId)
    {
        return await _context.PaymentTransactions
            .Include(x => x.UserSubscription)
            .ThenInclude(x => x.SubscriptionPlan)
            .Where(x => x.UserSubscription.UserId == userId)
            .OrderByDescending(x => x.ProcessedAt)
            .Select(x => new
            {
                x.Id,
                PlanName = x.UserSubscription.SubscriptionPlan.Name,
                x.Amount,
                x.Status,
                x.TransactionType,
                x.FailureReason,
                x.ProcessedAt
            })
            .ToListAsync();
    }

    public async Task<object?> GetBillingSummaryAsync(int userId)
    {
        var subscription = await _context.UserSubscriptions
            .Include(x => x.SubscriptionPlan)
            .FirstOrDefaultAsync(x => x.UserId == userId && x.BillingActive);

        if (subscription == null)
            return null;

        var user = await _context.Users.FindAsync(userId);

        return new
        {
            PlanName = subscription.SubscriptionPlan.Name,
            subscription.CurrentPrice,
            subscription.NextBillingDate,
            subscription.Status,
            BillingActive = subscription.BillingActive,
            AccountCredits = user?.AccountCredits ?? 0
        };
    }
    
    public async Task<string> SubscribeAsync(int subscriptionId)
    {
        var userIdClaim = _httpContextAccessor.HttpContext?
            .User?
            .FindFirst(ClaimTypes.NameIdentifier)?
            .Value;

        if (string.IsNullOrEmpty(userIdClaim))
            return "User not authenticated";

        var userId = int.Parse(userIdClaim);

        var user = await _context.Users
            .Include(x => x.UserSubscriptions)
            .FirstOrDefaultAsync(x => x.Id == userId);

        if (user == null)
            return "User not found";

        var plan = await _context.SubscriptionPlans.FindAsync(subscriptionId);

        if (plan == null)
            return "Subscription plan not found";

        if (plan.Price <= 0)
            return "Invalid subscription plan price";
        
        if (user.UserSubscriptions.Any(x => x is
            {
                BillingActive: true, 
                Status: SubscriptionStatus.Active
            }))
            return "User already has an active subscription";

        if (user.AccountCredits < plan.Price)
            return "Insufficient credit balance";

        user.AccountCredits -= plan.Price;

        var userSubscription = new UserSubscription
        {
            UserId = user.Id,
            SubscriptionPlanId = plan.Id,
            BillingActive = true,
            CurrentPrice = plan.Price,
            StartDate = DateTime.UtcNow,
            NextBillingDate = DateTime.UtcNow.AddMonths(1),
            Status = SubscriptionStatus.Active
        };

        _context.UserSubscriptions.Add(userSubscription);

        await _context.SaveChangesAsync();

        var paymentTransaction = new PaymentTransaction
        {
            UserSubscriptionId = userSubscription.Id,
            Amount = plan.Price,
            ProcessedAt = DateTime.UtcNow,
            Status = "Paid",
            TransactionType = "InitialCharge"
        };

        _context.PaymentTransactions.Add(paymentTransaction);

        await _context.SaveChangesAsync();

        return $"Subscribed to {plan.Name}";
    }
    
    public async Task<object?> CancelBillingAsync(int subscriptionId)
    {
        var sub = await _context.UserSubscriptions.FindAsync(subscriptionId);

        if (sub == null)
            return "Subscription not found";

        sub.BillingActive = false;
        sub.Status = SubscriptionStatus.Cancelled;

        await _context.SaveChangesAsync();

        return "Billing cancelled";
    }

    public async Task<object?> ResumeBillingAsync(int subscriptionId)
    {
        var sub = await _context.UserSubscriptions.FindAsync(subscriptionId);

        if (sub == null)
            return "Subscription not found";

        sub.BillingActive = true;
        sub.Status = SubscriptionStatus.Active;

        if (sub.NextBillingDate == null || sub.NextBillingDate < DateTime.UtcNow)
            sub.NextBillingDate = DateTime.UtcNow.AddMonths(1);

        await _context.SaveChangesAsync();

        return "Billing resumed";
    }
    
}