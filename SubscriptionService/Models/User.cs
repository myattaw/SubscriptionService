namespace SubscriptionService.Models;

public class User
{
    public int Id { get; set; }

    public string Email { get; set; } = "";

    public string PasswordHash { get; set; } = "";

    public string FullName { get; set; } = "";
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public List<UserSubscription> UserSubscriptions { get; set; } = new();
    
}