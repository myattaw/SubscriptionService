namespace SubscriptionService.Models.Requests;

using System.ComponentModel.DataAnnotations;

public record LoginRequest(
    [Required] string Email,
    [Required] string Password
);