namespace SubscriptionService.Models.Requests;

public record AssignSubscriptionRequest(int UserId, int SubscriptionPlanId);