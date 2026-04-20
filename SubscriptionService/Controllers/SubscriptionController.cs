using Microsoft.AspNetCore.Mvc;

namespace SubscriptionService.Controllers;

public class SubscriptionController : Controller
{
 
        [HttpGet("/subscriptions")]
        public IActionResult GetSubscriptions()
        {
            // Placeholder for fetching subscriptions from a data source
            var subscriptions = new[]
            {
                new { Id = 1, Name = "Basic Plan", Price = 9.99 },
                new { Id = 2, Name = "Premium Plan", Price = 19.99 }
            };
            return Ok(subscriptions);
        }
    
}