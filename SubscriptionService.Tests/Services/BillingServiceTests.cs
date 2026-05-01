using Microsoft.EntityFrameworkCore;
using SubscriptionService.Models;
using SubscriptionService.Models.Subscription;
using SubscriptionService.Services.Billing;
using SubscriptionService.Tests.Helpers;

namespace SubscriptionService.Tests.Services;

public class BillingServiceTests
{
    [Fact]
    public async Task SubscribeAsync_ShouldReturnUserNotAuthenticatedWhenNoClaim()
    {
        var context = TestDbFactory.Create();
        var httpAccessor = HttpContextMocker.CreateWithoutUser();

        var service = new BillingService(context, httpAccessor);

        var result = await service.SubscribeAsync(1);

        Assert.Equal("User not authenticated", result);
    }

    [Fact]
    public async Task SubscribeAsync_ShouldReturnUserNotFound()
    {
        var context = TestDbFactory.Create();
        var httpAccessor = HttpContextMocker.CreateWithUserId(999);

        var service = new BillingService(context, httpAccessor);

        var result = await service.SubscribeAsync(1);

        Assert.Equal("User not found", result);
    }

    [Fact]
    public async Task SubscribeAsync_ShouldReturnPlanNotFound()
    {
        var context = TestDbFactory.Create();

        context.Users.Add(new User
        {
            Email = "test@test.com",
            AccountCredits = 100
        });

        await context.SaveChangesAsync();

        var httpAccessor = HttpContextMocker.CreateWithUserId(1);

        var service = new BillingService(context, httpAccessor);

        var result = await service.SubscribeAsync(999);

        Assert.Equal("Subscription plan not found", result);
    }

    [Fact]
    public async Task SubscribeAsync_ShouldReturnInsufficientBalance()
    {
        var context = TestDbFactory.Create();

        context.Users.Add(new User
        {
            Email = "test@test.com",
            AccountCredits = 5
        });

        context.SubscriptionPlans.Add(new SubscriptionPlan
        {
            Name = "Premium",
            Price = 20
        });

        await context.SaveChangesAsync();

        var httpAccessor = HttpContextMocker.CreateWithUserId(1);

        var service = new BillingService(context, httpAccessor);

        var result = await service.SubscribeAsync(1);

        Assert.Equal("Insufficient credit balance", result);
    }

    [Fact]
    public async Task SubscribeAsync_ShouldCreateSubscriptionAndTransaction()
    {
        var context = TestDbFactory.Create();

        context.Users.Add(new User
        {
            Email = "test@test.com",
            AccountCredits = 100
        });

        context.SubscriptionPlans.Add(new SubscriptionPlan
        {
            Name = "Gold",
            Price = 25
        });

        await context.SaveChangesAsync();

        var httpAccessor = HttpContextMocker.CreateWithUserId(1);

        var service = new BillingService(context, httpAccessor);

        var result = await service.SubscribeAsync(1);

        Assert.Equal("Subscribed to Gold", result);

        var user = await context.Users.FirstAsync();
        Assert.Equal(75, user.AccountCredits);

        Assert.Single(context.UserSubscriptions);
        Assert.Single(context.PaymentTransactions);

        var payment = await context.PaymentTransactions.FirstAsync();
        Assert.Equal("Paid", payment.Status);
        Assert.Equal("InitialCharge", payment.TransactionType);
    }

    [Fact]
    public async Task SubscribeAsync_ShouldReturnAlreadyHasActiveSubscription()
    {
        var context = TestDbFactory.Create();

        var user = new User
        {
            Email = "test@test.com",
            AccountCredits = 100
        };

        var plan = new SubscriptionPlan
        {
            Name = "Gold",
            Price = 25
        };

        context.Users.Add(user);
        context.SubscriptionPlans.Add(plan);

        await context.SaveChangesAsync();

        context.UserSubscriptions.Add(new UserSubscription
        {
            UserId = user.Id,
            SubscriptionPlanId = plan.Id,
            BillingActive = true,
            CurrentPrice = 25,
            Status = SubscriptionStatus.Active,
            StartDate = DateTime.UtcNow,
            NextBillingDate = DateTime.UtcNow.AddMonths(1)
        });

        await context.SaveChangesAsync();

        var httpAccessor = HttpContextMocker.CreateWithUserId(user.Id);
        var service = new BillingService(context, httpAccessor);

        var result = await service.SubscribeAsync(plan.Id);

        Assert.Equal("User already has an active subscription", result);
    }

    [Fact]
    public async Task CancelBillingAsync_ShouldReturnNotFound()
    {
        var context = TestDbFactory.Create();
        var httpAccessor = HttpContextMocker.CreateWithoutUser();

        var service = new BillingService(context, httpAccessor);

        var result = await service.CancelBillingAsync(999);

        Assert.Equal("Subscription not found", result);
    }

    [Fact]
    public async Task CancelBillingAsync_ShouldCancelSubscription()
    {
        var context = TestDbFactory.Create();

        var user = new User { Email = "test@test.com", AccountCredits = 100 };
        var plan = new SubscriptionPlan { Name = "Gold", Price = 25 };

        context.Users.Add(user);
        context.SubscriptionPlans.Add(plan);
        await context.SaveChangesAsync();

        var sub = new UserSubscription
        {
            UserId = user.Id,
            SubscriptionPlanId = plan.Id,
            BillingActive = true,
            CurrentPrice = 25,
            Status = SubscriptionStatus.Active,
            StartDate = DateTime.UtcNow,
            NextBillingDate = DateTime.UtcNow.AddMonths(1)
        };

        context.UserSubscriptions.Add(sub);
        await context.SaveChangesAsync();

        var service = new BillingService(context, HttpContextMocker.CreateWithoutUser());

        var result = await service.CancelBillingAsync(sub.Id);

        Assert.Equal("Billing cancelled", result);

        var dbSub = await context.UserSubscriptions.FindAsync(sub.Id);
        Assert.False(dbSub!.BillingActive);
        Assert.Equal(SubscriptionStatus.Cancelled, dbSub.Status);
    }

    [Fact]
    public async Task ResumeBillingAsync_ShouldResumeSubscription()
    {
        var context = TestDbFactory.Create();

        var user = new User { Email = "test@test.com", AccountCredits = 100 };
        var plan = new SubscriptionPlan { Name = "Gold", Price = 25 };

        context.Users.Add(user);
        context.SubscriptionPlans.Add(plan);
        await context.SaveChangesAsync();

        var sub = new UserSubscription
        {
            UserId = user.Id,
            SubscriptionPlanId = plan.Id,
            BillingActive = false,
            CurrentPrice = 25,
            Status = SubscriptionStatus.Cancelled,
            StartDate = DateTime.UtcNow,
            NextBillingDate = DateTime.UtcNow.AddDays(-2)
        };

        context.UserSubscriptions.Add(sub);
        await context.SaveChangesAsync();

        var service = new BillingService(context, HttpContextMocker.CreateWithoutUser());

        var result = await service.ResumeBillingAsync(sub.Id);

        Assert.Equal("Billing resumed", result);

        var dbSub = await context.UserSubscriptions.FindAsync(sub.Id);

        Assert.True(dbSub!.BillingActive);
        Assert.Equal(SubscriptionStatus.Active, dbSub.Status);
        Assert.True(dbSub.NextBillingDate > DateTime.UtcNow);
    }
    
}