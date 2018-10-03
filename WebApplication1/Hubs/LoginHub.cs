using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace WebApplication1.Hubs
{
    public class LoginHub : Hub
    {
        [Authorize]
        public async Task SendLogoutMessage(string userId)
        {
            if (Clients == null)
                return;

            await Clients.All.SendAsync("UserLoggedOut");
        }

        [Authorize]
        public async Task SendLoginMessage(string userId)
        {
            if (Clients == null)
                return;

            await Clients.All.SendAsync("UserLoggedIn");
        }
    }
}
