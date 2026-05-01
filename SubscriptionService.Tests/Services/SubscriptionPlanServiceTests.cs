using SubscriptionService.Models.Requests;
using SubscriptionService.Services;
using SubscriptionService.Tests.Helpers;

namespace SubscriptionService.Tests.Services;

public class SubscriptionPlanServiceTests
{
    [Fact]
    public async Task CreatePlan_ShouldCreateNewPlan()
    {
        var context = TestDbFactory.Create();
        var service = new SubscriptionPlanService(context);

        var request = new CreateSubscriptionPlanRequest("Gold", 49.99m);

        var result = await service.CreatePlan(request);

        Assert.NotNull(result);
        Assert.Equal("Gold", result.Name);
        Assert.Equal(49.99m, result.Price);
        Assert.Single(context.SubscriptionPlans);
    }

    [Fact]
    public async Task GetAvailablePlans_ShouldReturnAllPlans()
    {
        var context = TestDbFactory.Create();

        context.SubscriptionPlans.Add(new()
        {
            Name = "Free",
            Price = 0
        });

        context.SubscriptionPlans.Add(new()
        {
            Name = "Premium",
            Price = 19.99m
        });

        await context.SaveChangesAsync();

        var service = new SubscriptionPlanService(context);

        var result = await service.GetAvailablePlans();

        Assert.Equal(2, result.Count);
        Assert.Contains(result, x => x.Name == "Free");
        Assert.Contains(result, x => x.Name == "Premium");
    }
    
}