using SubscriptionService.Models.Subscription;

namespace SubscriptionService.Models.Payment;

public class PaymentTransaction
{
    public int Id { get; set; }

    public int UserSubscriptionId { get; set; }

    public decimal Amount { get; set; }

    public DateTime ProcessedAt { get; set; } = DateTime.UtcNow;

    public string Status { get; set; } = string.Empty;
    // Paid / Failed

    public string TransactionType { get; set; } = string.Empty;
    // InitialCharge / RecurringCharge / FailedCharge

    public string? FailureReason { get; set; }

    public UserSubscription UserSubscription { get; set; } = null!;
}