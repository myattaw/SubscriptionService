using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SubscriptionService.Data;
using SubscriptionService.Models;

namespace SubscriptionService.Controllers;

[ApiController]
[Route("api/subscriptions")]
public class SubscriptionController : ControllerBase
{
    
    private readonly SubscriptionDbContext _context;
    
    public SubscriptionController(SubscriptionDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetSubscriptions()
    {
        var subs = await _context.Subscriptions.ToListAsync();
        return Ok(subs);
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetSubscription(int id)
    {
        var subscription = await _context.Subscriptions.FindAsync(id);

        if (subscription == null)
            return NotFound();

        return Ok(subscription);
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSubscription(int id)
    {
        var subscription = await _context.Subscriptions.FindAsync(id);

        if (subscription == null)
            return NotFound();

        _context.Subscriptions.Remove(subscription);
        await _context.SaveChangesAsync();

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