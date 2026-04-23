using Microsoft.AspNetCore.Mvc;
using SubscriptionService.Models;

namespace SubscriptionService.Controllers;

[ApiController]
[Route("api/subscriptions")]
public class SubscriptionController : ControllerBase
{
    private static List<Subscription> _subscriptions =
    [
        new(1, "Basic Plan", 9.99f),
        new(2, "Premium Plan", 19.99f)
    ];

    [HttpGet]
    public IActionResult GetSubscriptions()
    {
        return Ok(_subscriptions);
    }

    [HttpPost]
    public IActionResult CreateSubscription([FromBody] Subscription subscription)
    {
        _subscriptions.Add(subscription);
        return CreatedAtAction(nameof(GetSubscriptions), new { id = subscription.ID }, subscription);
    }
    
}