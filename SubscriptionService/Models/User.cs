namespace SubscriptionService.Models;

public class User
{
    public int Id { get; set; }

    public string Email { get; set; } = "";

    public string PasswordHash { get; set; } = "";

    public string FullName { get; set; } = "";
    
    // For simplicity, we use a decimal to represent account credit.
    // In a real application, you might want to use a more robust financial library.
    public decimal AccountCredits { get; set; } = 0.00m;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public List<UserSubscription> UserSubscriptions { get; set; } = new();
    
}