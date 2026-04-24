using Microsoft.AspNetCore.Mvc;
using SubscriptionService.Data;
using SubscriptionService.Models;

namespace SubscriptionService.Controllers;

[ApiController]
[Route("api/subscriptions")]
public class SubscriptionController : ControllerBase
{
    
    private readonly SubscriptionDbContext _context;
    
    private static List<Subscription> _subscriptions =
    [
        new(1, "Basic Plan", 9.99f),
        new(2, "Premium Plan", 19.99f)
    ];
    
    public SubscriptionController(SubscriptionDbContext context)
    {
        _context = context;
    }

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
    public async Task<IActionResult> CreateSubscription([FromBody] Subscription subscription)
    {
        _context.Subscriptions.Add(subscription);
        //TODO: implement service class to handle business logic and data access
        await _context.SaveChangesAsync();
        
        return CreatedAtAction(nameof(GetSubscriptions), new { id = subscription.ID }, subscription);
    }
    
}