namespace SubscriptionService.Models.Payment;

public class PaymentTransaction
{
    public int Id { get; set; }
    public int UserSubscriptionId { get; set; }
    public decimal Amount { get; set; }
    public DateTime ProcessedAt { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? FailureReason { get; set; }

    public UserSubscription UserSubscription { get; set; } = null!;
}