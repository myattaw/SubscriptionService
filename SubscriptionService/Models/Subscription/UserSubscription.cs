using SubscriptionService.Models.Payment;

namespace SubscriptionService.Models.Subscription;

public class UserSubscription
{
    public int Id { get; set; }

    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public int SubscriptionPlanId { get; set; }
    public SubscriptionPlan SubscriptionPlan { get; set; } = null!;

    public bool BillingActive { get; set; } = true;

    public decimal CurrentPrice { get; set; }

    public DateTime StartDate { get; set; } = DateTime.UtcNow;

    public DateTime? NextBillingDate { get; set; }

    public string Status { get; set; } = "Active";
    // Active / PastDue / Cancelled / Expired

    public List<PaymentTransaction> PaymentTransactions { get; set; } = new();
}