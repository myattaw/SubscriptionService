using Microsoft.EntityFrameworkCore;
using SubscriptionService.Data;
using SubscriptionService.Models;
using SubscriptionService.Models.Enums;
using SubscriptionService.Models.Subscription;
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

        options.Events.OnRedirectToLogin = context =>
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return Task.CompletedTask;
        };

        options.Events.OnRedirectToAccessDenied = context =>
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            return Task.CompletedTask;
        };
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
    
    
    if (!db.Users.Any(x => x.Email == "admin"))
    {
        db.Users.Add(new User
        {
            Email = "admin",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin"),
            Role = UserRole.Admin,
            AccountCredits = 1000
        });
        db.SaveChanges();
    }
}



app.UseAuthentication();
app.UseAuthorization();

app.UseHttpsRedirection();
app.MapControllers();

app.Run();