using Microsoft.EntityFrameworkCore;
using SubscriptionService.Data;
using SubscriptionService.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddDbContext<SubscriptionDbContext>(options =>
    options.UseSqlite("Data Source=subscriptions.db"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<SubscriptionDbContext>();
    db.Database.EnsureCreated();

    if (!db.Subscriptions.Any())
    {
        db.Subscriptions.AddRange(
            new Subscription { Name = "Basic Plan", Price = 9.99m },
            new Subscription { Name = "Premium Plan", Price = 19.99m }
        );

        db.SaveChanges();
    }
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();