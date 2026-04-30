using Microsoft.AspNetCore.Mvc;
using SubscriptionService.Services;

namespace SubscriptionService.Controllers;

[ApiController]
[Route("api/billing")]
public class BillingController : ControllerBase
{
    private readonly BillingService _billingService;

    public BillingController(BillingService billingService)
    {
        _billingService = billingService;
    }

    [HttpGet("history/{userId}")]
    public async Task<IActionResult> GetPaymentHistory(int userId)
    {
        var history = await _billingService.GetPaymentHistoryAsync(userId);
        return Ok(history);
    }

    [HttpGet("summary/{userId}")]
    public async Task<IActionResult> GetBillingSummary(int userId)
    {
        var summary = await _billingService.GetBillingSummaryAsync(userId);
        return Ok(summary);
    }

    [HttpPost("subscribe/{subscriptionId}")]
    public async Task<IActionResult> Subscribe(int subscriptionId)
    {
        var result = await _billingService.SubscribeAsync(subscriptionId);
        return Ok(result);
    }
    
    [HttpPost("cancel/{subscriptionId}")]
    public async Task<IActionResult> CancelBilling(int subscriptionId)
    {
        var result = await _billingService.CancelBillingAsync(subscriptionId);
        return Ok(result);
    }

    [HttpPost("resume/{subscriptionId}")]
    public async Task<IActionResult> ResumeBilling(int subscriptionId)
    {
        var result = await _billingService.ResumeBillingAsync(subscriptionId);
        return Ok(result);
    }
    
}