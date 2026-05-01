using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Moq;

namespace SubscriptionService.Tests.Helpers;

public static class HttpContextMocker
{
    public static IHttpContextAccessor CreateWithUserId(int userId)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId.ToString())
        };

        var identity = new ClaimsIdentity(claims, "TestAuth");
        var principal = new ClaimsPrincipal(identity);

        var context = new DefaultHttpContext
        {
            User = principal
        };

        var accessor = new Mock<IHttpContextAccessor>();
        accessor.Setup(x => x.HttpContext).Returns(context);

        return accessor.Object;
    }

    public static IHttpContextAccessor CreateWithoutUser()
    {
        var context = new DefaultHttpContext();

        var accessor = new Mock<IHttpContextAccessor>();
        accessor.Setup(x => x.HttpContext).Returns(context);

        return accessor.Object;
    }
    
}