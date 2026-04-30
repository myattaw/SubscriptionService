namespace SubscriptionService.Models.Responses;

public record SubscriptionPlanResponse(
    int Id,
    string Name,
    decimal Price
);