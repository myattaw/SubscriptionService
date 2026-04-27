using Microsoft.EntityFrameworkCore;
using SubscriptionService.Data;
using SubscriptionService.Models;

namespace SubscriptionService.Services;

public class AuthService
{
    private readonly AppDbContext _context;

    public AuthService(AppDbContext context)
    {
        _context = context;
    }
    
    public async Task<User?> ValidateUser(string email, string password)
    {
        var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == email);

        if (user == null)
            return null;

        var valid = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);

        return valid ? user : null;
    }

    public async Task<User> CreateUser(string email, string password)
    {
        var existing = await _context.Users.AnyAsync(x => x.Email == email);

        if (existing)
            throw new Exception("Email already in use");

        var user = new User
        {
            Email = email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password)
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return user;
    }

}