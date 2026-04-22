using Microsoft.AspNetCore.Mvc;
using SubscriptionService.Models;

namespace SubscriptionService.Controllers;

public class SubscriptionController : Controller
{
 
        [HttpGet("/subscriptions")]
        public IActionResult GetSubscriptions()
        {
            // Placeholder for fetching subscriptions from a data source
            var subscriptions = new Subscription[]
            {
                new(1, "Basic Plan", 9.99f),
                new(2, "Premium Plan", 19.99f)
            };
            
            return Ok(subscriptions);
        }
    
}