using SubscriptionService.Data;

namespace SubscriptionService.Services;

public class BillingService
{
    
    private readonly AppDbContext _context;

    public BillingService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<object?> GetPaymentHistoryAsync(int userId)
    {
        throw new NotImplementedException();
    }

    public async Task<object?> GetBillingSummaryAsync(int userId)
    {
        throw new NotImplementedException();
    }
    
    public async Task<object?> CancelBillingAsync(int subscriptionId)
    {
        throw new NotImplementedException();
    }

    public async Task<object?> ResumeBillingAsync(int subscriptionId)
    {
        throw new NotImplementedException();
    }
    
}