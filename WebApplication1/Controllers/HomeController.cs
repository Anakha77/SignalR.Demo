using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using WebApplication1.Hubs;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHubContext<LoginHub> _hubContext;
        private readonly bool _filteringCookies;

        public HomeController(IHubContext<LoginHub> hubContext)
        {
            _hubContext = hubContext;
            _filteringCookies = false;
        }

        public IActionResult Index()
        {
            var cookies = HttpContext.Request.Cookies.AsEnumerable();
            var cookieData = new Dictionary<string, string>();

            if (_filteringCookies)
                cookies = cookies.Where(c => c.Key.StartsWith("WebApplication.Cookies"));

            foreach (var cookie in cookies)
            {
                cookieData.Add(cookie.Key, cookie.Value);
            }

            return View(new LoginViewModel { Cookies = cookieData });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginInputModel model)
        {
            //Authentication
            var props = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.Add(TimeSpan.FromDays(30))
            };
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, model.Username),
                new Claim(ClaimTypes.NameIdentifier, model.Username)
            };
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), props);

            //set cookies
            var cookieOptions = new CookieOptions
            {
                Domain = "uc1083.local",
                MaxAge = TimeSpan.FromHours(1)
            };
            Response.Cookies.Append("WebApplication.Cookies.Shared", model.Username, cookieOptions);

            await _hubContext.Clients.User(model.Username).SendAsync("UserLoggedIn");

            // return view
            return Redirect(nameof(Index));
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            Response.Cookies.Delete("WebApplication.Cookies.Shared", new CookieOptions { Domain = "uc1083.local" });

            await HttpContext.SignOutAsync();

            await _hubContext.Clients.User(User.Claims.Single(c => c.Type == ClaimTypes.NameIdentifier).Value).SendAsync("UserLoggedOut");

            return Redirect(nameof(Index));
        }
    }
}