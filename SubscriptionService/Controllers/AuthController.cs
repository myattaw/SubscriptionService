using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SubscriptionService.Data;
using SubscriptionService.Models;
using SubscriptionService.Services;

namespace SubscriptionService.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    
    private readonly AuthService _authService;

    public AuthController(AuthService authService)
    {
        _authService = authService;
    }
    
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var user = await _authService.ValidateUser(request.Email, request.Password);

        if (user == null)
            return Unauthorized();

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email)
        };

        var identity = new ClaimsIdentity(claims, "AppCookie");
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync("AppCookie", principal);

        return Ok();
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        try
        {
            var user = await _authService.CreateUser(request.Email, request.Password);

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Email, user.Email)
            };

            var identity = new ClaimsIdentity(claims, "AppCookie");
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync("AppCookie", principal);

            return Ok(new { user.Id, user.Email });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
    
}