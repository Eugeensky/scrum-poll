using DAL;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

namespace ScrumPoll.Services;
public class AuthorizationService
{
    private readonly Database db;
    private readonly IHttpContextAccessor httpContextAccessor;
    public AuthorizationService(Database _db, IHttpContextAccessor _httpContextAccessor)
    {
        db = _db;
        httpContextAccessor = _httpContextAccessor;
    }

    public async Task<(bool success, string message)> Authorize(string login, string password)
    {
        var context = httpContextAccessor.HttpContext;
        if (context is null) return (false, "Http context error");
        var user = await db.User.GetVerifiedUser(login.ToLower(), password);

        if (user is null)
        {
            return (false, "The email and password combination is not valid.");
        }
        var claims = new List<Claim>
        {
            new Claim(ClaimsIdentity.DefaultNameClaimType, user.Login),
            new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Role)
        };
        var claimsIdentity = new ClaimsIdentity(claims, "Cookies");
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
        await context.SignInAsync(claimsPrincipal);
        return (true, string.Empty);
    }

    public async Task Logout()
    {
        var context = httpContextAccessor.HttpContext;
        if (context is not null)
        {
            await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }
    }
}
