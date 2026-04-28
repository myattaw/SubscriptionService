namespace SubscriptionService.Models;

public class SubscriptionPlan
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public decimal Price { get; set; }

    public List<UserSubscription> UserSubscriptions { get; set; } = new();
}