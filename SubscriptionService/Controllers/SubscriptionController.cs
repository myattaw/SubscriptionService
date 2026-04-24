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
    
    [HttpGet("{id}")]
    public IActionResult GetSubscription(int id)
    {
        var subscription = _subscriptions.FirstOrDefault(s => s.ID == id);
        if (subscription == null)
        {
            return NotFound();
        }
        return Ok(subscription);
    }
    
    [HttpDelete("{id}")]
    public IActionResult DeleteSubscription(int id)
    {
        var subscription = _subscriptions.FirstOrDefault(s => s.ID == id);
        if (subscription == null)
        {
            return NotFound();
        }
        _subscriptions.Remove(subscription);
        return NoContent();
    }
    
    [HttpPost]
    public IActionResult CreateSubscription([FromBody] Subscription subscription)
    {
        _subscriptions.Add(subscription);
        return CreatedAtAction(nameof(GetSubscriptions), new { id = subscription.ID }, subscription);
    }
    
}