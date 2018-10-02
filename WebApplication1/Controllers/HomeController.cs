using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using WebApplication1.Hubs;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHubContext<LoginHub> _hubContext;

        public HomeController(IHubContext<LoginHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public IActionResult Index()
        {
            var cookies = HttpContext.Request.Cookies;
            var cookieData = new Dictionary<string, string>();
            foreach (var cookie in cookies.Where(c => c.Key.StartsWith("WebApplication.Cookies")))
            {
                cookieData.Add(cookie.Key, cookie.Value);
            }

            return View(cookieData);
        }

        [HttpPost]
        public async Task<IActionResult> Login()
        {
            //generage data
            var crypto = new System.Security.Cryptography.SHA256CryptoServiceProvider();
            var byteConverter = new UnicodeEncoding();
            var dataToEncrypt = byteConverter.GetBytes("secret");
            var cryptedData = Convert.ToBase64String(crypto.ComputeHash(dataToEncrypt));

            //set cookies
            var cookieOptions = new CookieOptions
            {
                Domain = "uc1083.local",
                MaxAge = TimeSpan.FromHours(1)
            };
            Response.Cookies.Append("WebApplication.Cookies.Shared", "public data", cookieOptions);


            var privateCookieOptions = new CookieOptions
            {
                Domain = "prive.uc1083.local",
                HttpOnly = true,
                SameSite = SameSiteMode.Lax,
                MaxAge = TimeSpan.FromHours(1)
            };
            Response.Cookies.Append("WebApplication.Cookies.Private", cryptedData, privateCookieOptions);

            await _hubContext.Clients.All.SendAsync("UserLoggedIn");

            // return view
            return Redirect(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            Response.Cookies.Delete("WebApplication.Cookies.Shared", new CookieOptions { Domain = "uc1083.local" });
            Response.Cookies.Delete("WebApplication.Cookies.Private", new CookieOptions { Domain = "prive.uc1083.local" });

            await _hubContext.Clients.All.SendAsync("UserLoggedOut");

            return Redirect(nameof(Index));
        }
    }
}