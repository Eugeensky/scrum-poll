using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using ScrumPoll.Services;

namespace ScrumPoll.Controllers;
public class LoginController : Controller
{
    private readonly AuthorizationService authorizationService;
    public LoginController(AuthorizationService _authorizationService)
    {
        authorizationService = _authorizationService;
    }
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        await authorizationService.Logout();
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> Authorize(string login, string password)
    {
        var (success, message) = await authorizationService.Authorize(login, password);
        if (!success)
        {
            ViewBag.Message = message;
            return View("Index");
        }
        return RedirectToAction("Index", "Poll");
    }
}
