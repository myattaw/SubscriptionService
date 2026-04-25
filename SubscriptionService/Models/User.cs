namespace SubscriptionService.Models;

public class User
{
    public int Id { get; set; }

    public string Email { get; set; } = "";

    public string PasswordHash { get; set; } = "";

    public string FullName { get; set; } = "";

    public string SubscriptionTier { get; set; } = "Free";

    public bool BillingActive { get; set; } = false;
    
    public DateTime? NextBillingDate { get; set; } = null;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public List<Subscription> Subscriptions { get; set; } = new();
    
}