namespace SubscriptionService.Services;

public class BillingCycleService : BackgroundService
{
    
    private readonly ILogger<BillingCycleService> _logger;

    public BillingCycleService(ILogger<BillingCycleService> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {

            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

            await RunBillingCycle();
            
            await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
        }
    }

    private async Task RunBillingCycle()
    {
        throw new NotImplementedException();
    }
    
    
    
}