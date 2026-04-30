using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SubscriptionService.Models.Requests;

namespace SubscriptionService.Controllers;

[ApiController]
[Route("api/subscriptions")]
public class SubscriptionController : ControllerBase
{
    private readonly Services.SubscriptionPlanService _subscriptionPlanService;

    public SubscriptionController(Services.SubscriptionPlanService subscriptionPlanService)
    {
        _subscriptionPlanService = subscriptionPlanService;
    }

    [HttpGet("plans")]
    public async Task<IActionResult> GetAvailablePlans()
    {
        var plans = await _subscriptionPlanService.GetAvailablePlans();
        return Ok(plans);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("plans")]
    public async Task<IActionResult> CreatePlan([FromBody] CreateSubscriptionPlanRequest request)
    {
        var plan = await _subscriptionPlanService.CreatePlan(request);
        return Ok(plan);
    }

    [HttpDelete("plans/{id}")]
    public async Task<IActionResult> DeletePlan(int id)
    {
        var deleted = await _subscriptionPlanService.DeletePlan(id);

        if (!deleted)
            return NotFound();

        return NoContent();
    }

    [HttpGet("active")]
    public async Task<IActionResult> GetActiveSubscriptions()
    {
        var activeSubscriptions = await _subscriptionPlanService.GetActiveSubscriptions();
        return Ok(activeSubscriptions);
    }

    [HttpPost("assign")]
    public async Task<IActionResult> AssignSubscription([FromBody] AssignSubscriptionRequest request)
    {
        var result = await _subscriptionPlanService.AssignSubscription(request);

        if (!result.Success)
            return BadRequest(result.Message);

        return Ok(result.Subscription);
    }
    
}