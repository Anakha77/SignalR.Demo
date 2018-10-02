using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication2.Controllers
{
    public class HomeController : Controller
    {
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
    }
}
