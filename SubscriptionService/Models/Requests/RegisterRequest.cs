namespace SubscriptionService.Models.Requests;

using System.ComponentModel.DataAnnotations;

public record RegisterRequest(
    [Required] string Email,
    [Required] string Password
);