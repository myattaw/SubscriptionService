using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SubscriptionService.Data;
using SubscriptionService.Models;
using SubscriptionService.Models.Requests;

namespace SubscriptionService.Controllers;

[ApiController]
[Route("api/subscriptions")]
public class SubscriptionController : ControllerBase
{
    private readonly AppDbContext _context;

    public SubscriptionController(AppDbContext context)
    {
        _context = context;
    }

    // =========================
    // SUBSCRIPTION PLAN CRUD
    // =========================

    [HttpGet("plans")]
    public async Task<IActionResult> GetAvailablePlans()
    {
        var plans = await _context.SubscriptionPlans.ToListAsync();
        return Ok(plans);
    }

    [HttpPost("plans")]
    public async Task<IActionResult> CreatePlan([FromBody] CreateSubscriptionPlanRequest request)
    {
        var plan = new SubscriptionPlan
        {
            Name = request.Name,
            Price = request.Price
        };

        _context.SubscriptionPlans.Add(plan);
        await _context.SaveChangesAsync();

        return Ok(plan);
    }

    [HttpDelete("plans/{id}")]
    public async Task<IActionResult> DeletePlan(int id)
    {
        var plan = await _context.SubscriptionPlans.FindAsync(id);

        if (plan == null)
            return NotFound();

        _context.SubscriptionPlans.Remove(plan);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // =========================
    // USER SUBSCRIPTION ASSIGNMENTS
    // =========================

    [HttpGet("active")]
    public async Task<IActionResult> GetActiveSubscriptions()
    {
        var activeSubscriptions = await _context.UserSubscriptions
            .Include(x => x.User)
            .Include(x => x.SubscriptionPlan)
            .ToListAsync();

        return Ok(activeSubscriptions);
    }

    [HttpPost("assign")]
    public async Task<IActionResult> AssignSubscription([FromBody] AssignSubscriptionRequest request)
    {
        var user = await _context.Users.FindAsync(request.UserId);
        if (user == null)
            return BadRequest("User does not exist.");

        var plan = await _context.SubscriptionPlans.FindAsync(request.SubscriptionPlanId);
        if (plan == null)
            return BadRequest("Subscription plan does not exist.");

        var alreadyAssigned = await _context.UserSubscriptions
            .AnyAsync(x => x.UserId == request.UserId && x.SubscriptionPlanId == request.SubscriptionPlanId);

        if (alreadyAssigned)
            return BadRequest("User already has this subscription.");

        var userSubscription = new UserSubscription
        {
            UserId = request.UserId,
            SubscriptionPlanId = request.SubscriptionPlanId,
            BillingActive = true,
            StartDate = DateTime.UtcNow,
            NextBillingDate = DateTime.UtcNow.AddMonths(1),
            Status = "Active"
        };

        _context.UserSubscriptions.Add(userSubscription);
        await _context.SaveChangesAsync();

        return Ok(userSubscription);
    }
    
}