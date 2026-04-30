using Microsoft.EntityFrameworkCore;
using SubscriptionService.Data;
using SubscriptionService.Models;
using SubscriptionService.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<SubscriptionPlanService>();
builder.Services.AddScoped<BillingService>();

builder.Services.AddHostedService<BillingCycleService>();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=subscriptions.db"));

builder.Services.AddAuthentication("AppCookie")
    .AddCookie("AppCookie", options =>
    {
        options.Cookie.Name = "AppCookie";
        options.LoginPath = "/api/auth/login";
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();

    if (!db.SubscriptionPlans.Any())
    {
        db.SubscriptionPlans.AddRange(
            new SubscriptionPlan
            {
                Name = "Free",
                Price = 0.00m
            },
            new SubscriptionPlan
            {
                Name = "Premium",
                Price = 19.99m
            }
        );

        db.SaveChanges();
    }
}

app.UseAuthentication();
app.UseAuthorization();

app.UseHttpsRedirection();
app.MapControllers();

app.Run();