using Microsoft.EntityFrameworkCore;
using SubscriptionService.Data;
using SubscriptionService.Models;
using SubscriptionService.Models.Requests;

namespace SubscriptionService.Services;

public class SubscriptionPlanService
{
    private readonly AppDbContext _context;

    public SubscriptionPlanService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<object>> GetAvailablePlans()
    {
        return await _context.SubscriptionPlans
            .AsNoTracking()
            .Select(x => new
            {
                x.Id,
                x.Name,
                x.Price
            })
            .Cast<object>()
            .ToListAsync();
    }

    public async Task<SubscriptionPlan> CreatePlan(CreateSubscriptionPlanRequest request)
    {
        var plan = new SubscriptionPlan
        {
            Name = request.Name,
            Price = request.Price
        };

        _context.SubscriptionPlans.Add(plan);
        await _context.SaveChangesAsync();

        return plan;
    }

    public async Task<bool> DeletePlan(int id)
    {
        var plan = await _context.SubscriptionPlans.FindAsync(id);

        if (plan == null)
            return false;

        _context.SubscriptionPlans.Remove(plan);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<List<object>> GetActiveSubscriptions()
    {
        return await _context.UserSubscriptions
            .AsNoTracking()
            .Select(x => new
            {
                AssignmentId = x.Id,
                UserId = x.UserId,
                UserEmail = x.User.Email,
                PlanId = x.SubscriptionPlanId,
                PlanName = x.SubscriptionPlan.Name,
                Price = x.SubscriptionPlan.Price,
                x.BillingActive,
                x.StartDate,
                x.NextBillingDate,
                x.Status
            })
            .Cast<object>()
            .ToListAsync();
    }

    public async Task<(bool Success, string Message, UserSubscription? Subscription)> AssignSubscription(AssignSubscriptionRequest request)
    {
        var userExists = await _context.Users.AnyAsync(x => x.Id == request.UserId);
        if (!userExists)
            return (false, "User does not exist.", null);

        var planExists = await _context.SubscriptionPlans.AnyAsync(x => x.Id == request.SubscriptionPlanId);
        if (!planExists)
            return (false, "Subscription plan does not exist.", null);

        var alreadyAssigned = await _context.UserSubscriptions
            .AnyAsync(x => x.UserId == request.UserId && x.SubscriptionPlanId == request.SubscriptionPlanId);

        if (alreadyAssigned)
            return (false, "User already has this subscription.", null);

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

        return (true, "Assigned successfully.", userSubscription);
    }
}